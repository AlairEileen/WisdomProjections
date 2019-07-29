using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisdomProjections.Views;

namespace WisdomProjections.Data_Executor
{
    public class IpCameraDataExecutor
    {
        private static IpCameraDataExecutor _ipCameraDe;
        private ImageFactoryView imgContainer;
        private IpCameraViewer ipCameraViewer;

        public static IpCameraDataExecutor IpCameraDe => _ipCameraDe ?? (_ipCameraDe = new IpCameraDataExecutor());

        public void InitContent(ImageFactoryView imageFactoryView)
        {
            imgContainer = imageFactoryView;
            ipCameraViewer?.Stop();
            ipCameraViewer = new IpCameraViewer(ApplicationInfoContext.IpCameraInfo, imgContainer.img);
        }

        public void Close(object sender, CancelEventArgs e)
        {
            ipCameraViewer?.Stop();
        }
    }
}
