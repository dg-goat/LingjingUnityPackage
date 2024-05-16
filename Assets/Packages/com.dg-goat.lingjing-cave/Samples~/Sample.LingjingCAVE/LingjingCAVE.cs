using UnityEngine;

namespace DG.GOAT.Lingjing.CAVE
{
    public class LingjingCAVE : MonoBehaviour
    {
        public GameObject m_CamerasRoot;
        public Camera[] m_Cameras;
        public bool m_EnableDebugQuad;
        public CAVEConfig m_CAVEConfig;
    
        private CAVEController m_caveController;
    
        // Start is called before the first frame update
        void Start()
        {
            m_caveController = gameObject.AddComponent<CAVEController>();
            m_caveController.Initialize(m_CAVEConfig);

            var count = m_Cameras.Length;
            for (int nIdx = 0; nIdx < count; nIdx++)
            {
                m_caveController.RegisterCamera(nIdx, m_Cameras[nIdx]);
            }
        }

        // Update is called once per frame
        void Update()
        {
            m_caveController.UpdateObserverPos(m_CamerasRoot.transform.localPosition);
            m_caveController.EnableDebugQuad(m_EnableDebugQuad);
        }
    }
}

