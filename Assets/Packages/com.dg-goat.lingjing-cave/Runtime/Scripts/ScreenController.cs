using UnityEngine;

namespace DG.GOAT.Lingjing.CAVE
{
    /// <summary>
    /// 屏幕控制器
    /// 根据屏幕配置生成屏幕、观察点物体
    /// 传入屏幕对应的相机组件
    /// 更新观察点位置
    ///   同时更新相机组件参数
    ///   可选择是否同时更新相机位置，更新世界坐标/本地坐标
    /// </summary>
    public class ScreenController : MonoBehaviour
    {
        private GameObject m_screen;
        private GameObject[] m_screenEdges;
        private GameObject m_observer;
        private Matrix4x4 m_m4Space2Screen;
        private GameObject m_debugQuad;

        private Camera m_camera;
        private bool m_enablePosUpdate;
        private bool m_isLocalPos;
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="screenCfg">单屏幕配置</param>
        public void Initialize(ScreenConfig screenCfg)
        {
            if (null == screenCfg)
            {
                Debug.LogError("ScreenController.Initialize 传入配置为空");
                return;
            }

            m_screen = CreateScreen(gameObject, screenCfg);
            
            m_screenEdges = new GameObject[4];
            m_screenEdges[0] = CreateScreenEdge(m_screen, "TopCenter", new Vector3(0, 0.5f, 0));
            m_screenEdges[1] = CreateScreenEdge(m_screen, "RightMiddle", new Vector3(0.5f, 0, 0));
            m_screenEdges[2] = CreateScreenEdge(m_screen, "BottomCenter", new Vector3(0, -0.5f, 0));
            m_screenEdges[3] = CreateScreenEdge(m_screen, "LeftMiddle", new Vector3(-0.5f, 0, 0));

            m_observer = CreateObserver(m_screen);

            m_m4Space2Screen = transform.worldToLocalMatrix.inverse * m_screen.transform.worldToLocalMatrix;
            
            m_debugQuad = CreateDebugQuad(m_screen);
            m_debugQuad.SetActive(false);

            return;

            GameObject CreateScreen(GameObject parent, ScreenConfig screenCfg)
            {
                var screen = new GameObject("Screen");
                
                screen.transform.SetParent(parent.transform);
                screen.transform.localPosition = screenCfg.m_Position;
                screen.transform.localRotation = Quaternion.Euler(screenCfg.m_Rotation);
                screen.transform.localScale = screenCfg.m_Scale;
                
                return screen;
            }

            GameObject CreateScreenEdge(GameObject parent, string name, Vector3 position)
            {
                var edge = new GameObject(name);
                
                edge.transform.SetParent(parent.transform);
                edge.transform.localPosition = position;
                
                return edge;
            }

            GameObject CreateObserver(GameObject parent)
            {
                var observer = new GameObject("Observer");
                
                observer.transform.SetParent(parent.transform);
                observer.transform.localPosition = Vector3.up;
                observer.transform.localRotation = Quaternion.identity;
                
                return observer;
            }

            GameObject CreateDebugQuad(GameObject parent)
            {
                var debugQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                
                debugQuad.name = "DebugQuad";
                debugQuad.transform.SetParent(parent.transform);
                debugQuad.transform.localPosition = Vector3.zero;
                debugQuad.transform.localRotation = Quaternion.identity;//.Euler(Vector3.zero);
                debugQuad.transform.localScale = Vector3.one;
                debugQuad.layer = LayerMask.NameToLayer("TransparentFX");
                
                return debugQuad;
            }
        }
        
        /// <summary>
        /// 注册相机
        /// </summary>
        /// <param name="camera">相机组件</param>
        /// <param name="enablePosUpdate">是否更新相机位置</param>
        /// <param name="isLocalPos">是否以本地坐标更新相机位置</param>
        public void RegisterCamera(Camera camera, bool enablePosUpdate = false, bool isLocalPos = true)
        {
            m_camera = camera;
            m_enablePosUpdate = enablePosUpdate;
            m_isLocalPos = isLocalPos;
        }

        /// <summary>
        /// 更新观察点位置
        /// 如果使能了enablePosUpdate，则根据观察点位置更新相机位置
        /// 此位置为观察点相对于空间原点的位置
        /// </summary>
        /// <param name="position">位置坐标</param>
        public void UpdateObserverPos(Vector3 position)
        {
            // 更新观察点位置 
            m_observer.transform.localPosition = m_m4Space2Screen.MultiplyPoint(position);

            if (null == m_camera) return;

            // 更新相机位置
            if (m_enablePosUpdate)
            {
                if (m_isLocalPos) m_camera.transform.localPosition = position;
                else m_camera.transform.position = m_observer.transform.position;                                      
            }
            
            // 更新相机FOV和投影矩阵
            {
                var bottomToTop = m_screenEdges[0].transform.position - m_screenEdges[2].transform.position;
                var leftToRight = m_screenEdges[1].transform.position - m_screenEdges[3].transform.position;
                var observerPos = m_observer.transform.localPosition;
                
                //Set FOV
                m_camera.ResetProjectionMatrix();
                m_camera.fieldOfView = -2 * Mathf.Rad2Deg * Mathf.Atan(bottomToTop.magnitude / 2 / (observerPos.z * m_observer.transform.parent.lossyScale.z));

                //Set the orientation
                var obV = observerPos.y * m_observer.transform.parent.lossyScale.y / (bottomToTop.magnitude / 2);
                var obH = observerPos.x * m_observer.transform.parent.lossyScale.x / (leftToRight.magnitude / 2);
                SetObliqueness(-obH, -obV, m_camera);    
            }
        
            return;
            
            void SetObliqueness(float horizObl, float vertObl, Camera cam)
            {
                Matrix4x4 mat = cam.projectionMatrix;
                mat[0, 2] = horizObl;
                mat[1, 2] = vertObl;
                cam.projectionMatrix = mat;
            }
        }

        /// <summary>
        /// 是否显示调试面片，用于查看屏幕的显示平面
        /// </summary>
        /// <param name="enable"></param>
        public void EnableDebugQuad(bool enable)
        {
            m_debugQuad.SetActive(enable);
        }
    }
}