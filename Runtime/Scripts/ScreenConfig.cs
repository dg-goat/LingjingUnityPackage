using System;
using UnityEngine;

namespace DG.GOAT.Lingjing.CAVE
{
    /// <summary>
    /// 屏幕配置，代表一个矩形的显示平面
    /// </summary>
    [Serializable]
    public class ScreenConfig
    {
        /// <summary>
        /// 位置：锚点位于屏幕中心
        /// </summary>
        public Vector3 m_Position;
        
        /// <summary>
        /// 旋转：默认正面朝向
        /// </summary>
        public Vector3 m_Rotation;
        
        /// <summary>
        /// 缩放：根据屏幕大小设置
        /// </summary>
        public Vector3 m_Scale;
    }
}