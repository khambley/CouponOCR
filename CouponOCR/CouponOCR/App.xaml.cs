using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CouponOCR
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new CouponsPage())
            {
                BarBackgroundColor = Color.FromHex("#03A9F4"),
                BarTextColor = Color.White
            };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
