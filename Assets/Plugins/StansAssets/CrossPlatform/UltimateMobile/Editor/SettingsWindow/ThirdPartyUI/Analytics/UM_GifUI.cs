using UnityEngine;
using SA.Foundation.Editor;

namespace SA.CrossPlatform
{
    public class UM_GifUI : UM_PluginSettingsUI
    {
        private UM_AnalyticsResolver m_serviceResolver;
        public override void OnAwake() {
            base.OnAwake();
            AddFeatureUrl("Getting Started", "https://unionassets.com/ultimate-mobile-pro/getting-started-807");
        }

        public override string Title {
            get {
                return "Gif Record & Share";
            }
        }

        public override string Description {
            get { return "Service allows you to record your gameplay as a GIF image and the share it"; }
        }

        protected override Texture2D Icon {
            get {
                return UM_Skin.GetServiceIcon("um_gif_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                if (m_serviceResolver == null) 
                {
                    m_serviceResolver = new UM_AnalyticsResolver();
                }

                return m_serviceResolver;
            }
        }

        protected override void OnServiceUI()
        {
           
        }
    }
}