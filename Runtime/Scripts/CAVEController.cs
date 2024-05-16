using UnityEngine;

namespace DG.GOAT.Lingjing.CAVE
{
    /// <summary>
    /// CAVE控制器
    /// 根据CAVE配置生成多个屏幕控制器
    /// 传入屏幕对应的相机组件
    /// 更新观察点位置
    ///   同时更新相机组件参数
    ///   可选择是否同时更新相机位置，更新世界坐标/本地坐标
    /// </summary>
    public class CAVEController : MonoBehaviour
    {
        private ScreenController[] m_screenCtrls;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="caveCfg">CAVE配置</param>
        public void Initialize(CAVEConfig caveCfg)
        {
            if (null == caveCfg || null == caveCfg.m_ScreenConfigs)
            {
                Debug.LogError("CAVEController.Initialize 传入配置为空");
                return;
            }

            var count = caveCfg.m_ScreenConfigs.Length;
            m_screenCtrls = new ScreenController[count];
            for (int nIdx = 0; nIdx < count; nIdx++)
            {
                var cfg = caveCfg.m_ScreenConfigs[nIdx];
                if (null == cfg)
                {
                    Debug.LogError($"CAVEController.Initialize 配置{nIdx}为空");
                    continue;
                }

                m_screenCtrls[nIdx] = CreateScreenController(gameObject, cfg);
                m_screenCtrls[nIdx].gameObject.name = $"ScreenCtrl_{nIdx}";
            }

            return;

            ScreenController CreateScreenController(GameObject parent, ScreenConfig screenCfg)
            {
                var obj = new GameObject();
                
                obj.transform.parent = parent.transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;

                var ctrl = obj.AddComponent<ScreenController>();
                ctrl.Initialize(screenCfg);
                
                return ctrl;
            }
        }

        /// <summary>
        /// 注册相机
        /// </summary>
        /// <param name="screenIdx">屏幕索引</param>
        /// <param name="camera">相机组件</param>
        /// <param name="enablePosUpdate">是否更新相机位置</param>
        /// <param name="isLocalPos">是否以本地坐标更新相机位置</param>
        public void RegisterCamera(int screenIdx, Camera camera, bool enablePosUpdate = false, bool isLocalPos = true)
        {
            if (null == m_screenCtrls) return;

            if (screenIdx < 0 || screenIdx >= m_screenCtrls.Length)
            {
                Debug.LogError($"CAVEController.RegisterCamera screenIdx={screenIdx}越界");
                return;
            }

            var screenCtrl = m_screenCtrls[screenIdx];
            if (null == screenCtrl) return;
            
            screenCtrl.RegisterCamera(camera, enablePosUpdate, isLocalPos);
        }
        /// <summary>
        /// 更新观察点位置
        /// 如果使能了enablePosUpdate，则根据观察点位置更新相机位置
        /// 此位置为观察点相对于空间原点的位置
        /// </summary>
        /// <param name="position">位置坐标</param>
        public void UpdateObserverPos(Vector3 position)
        {
            if (null == m_screenCtrls) return;

            var count = m_screenCtrls.Length;
            for (int nIdx = 0; nIdx < count; nIdx++)
            {
                if (null == m_screenCtrls[nIdx]) continue;
                
                m_screenCtrls[nIdx].UpdateObserverPos(position);
            }
        }
        
        /// <summary>
        /// 是否显示调试面片，用于查看屏幕的显示平面
        /// </summary>
        /// <param name="enable"></param>
        public void EnableDebugQuad(bool enable)
        {
            if (null == m_screenCtrls) return;

            var count = m_screenCtrls.Length;
            for (int nIdx = 0; nIdx < count; nIdx++)
            {
                if (null == m_screenCtrls[nIdx]) continue;
                
                m_screenCtrls[nIdx].EnableDebugQuad(enable);
            }
        }
    }
}

