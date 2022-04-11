using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CouponOCR
{
    public class CouponsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Coupon> Coupons { get; } = new ObservableCollection<Coupon>();

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        bool busy;
        public bool IsBusy
        {
            get { return busy; }
            set
            {
                if (busy == value)
                    return;

                busy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Message));
            }
        }

        public string Message { get; set; } = "Hello World";

        Command addCouponCommand = null;

        public Command AddCouponCommand => addCouponCommand ?? (addCouponCommand = new Command(async () => await ExecuteAddCouponCommandAsync()));

        async Task ExecuteAddCouponCommandAsync()
        {
            try
            {
                IsBusy = true;
                // 1. Add camera logic to take photo of coupon.


            }
            catch
            {

            }
        }
    }
}
