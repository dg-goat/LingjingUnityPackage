using UnityEngine;

namespace DG.GOAT.Lingjing.CAVE
{
    public class SingleScreenCAVE : MonoBehaviour
    {
        public Camera m_Camera;
        public ScreenConfig m_ScreenConfig;
        public bool m_EnableDebugQuad;

        private ScreenController m_screenCtrl;
        
        // Start is called before the first frame update
        void Start()
        {
            m_screenCtrl = gameObject.AddComponent<ScreenController>();
            m_screenCtrl.Initialize(m_ScreenConfig);
            m_screenCtrl.RegisterCamera(m_Camera, false, false);
        }

        // Update is called once per frame
        void Update()
        {
            m_screenCtrl.UpdateObserverPos(m_Camera.transform.localPosition);
            m_screenCtrl.EnableDebugQuad(m_EnableDebugQuad);
        }
    }
}

