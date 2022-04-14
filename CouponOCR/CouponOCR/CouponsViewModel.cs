using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CouponOCR
{
    public class CouponsViewModel : INotifyPropertyChanged
    {
        static string subscriptionKey = "584f3573f45847c6a772efa420bd68e4";
        static string endpoint = "https://couponocrapp.cognitiveservices.azure.com/";
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

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
            {
                Endpoint = endpoint
            };
            return client;
        }

        async Task ExecuteAddCouponCommandAsync()
        {
            try
            {
                IsBusy = true;
                // 1. Add camera logic to take photo of coupon.

                // Initialize all camera components, must be called before checking properties below
                await CrossMedia.Current.Initialize();

                MediaFile photo;
                if (CrossMedia.Current.IsCameraAvailable)
                {
                    // takes a photo with specified options
                    photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "Coupons",
                        Name = "Coupon"
                    });
                }
                else
                {
                    // picks a photo from default gallery
                    photo = await CrossMedia.Current.PickPhotoAsync();
                }

                if(photo == null)
                {
                    PrintStatus("Photo was null :(");
                    return;
                }

                // 2. Add OCR logic.

                

                var client = Authenticate(subscriptionKey, endpoint);

                using (var stream = photo.GetStream())
                {
                    var text = await client.ReadInStreamAsync(stream);

                    //after the request, get the operation location
                    string operationLocation = text.OperationLocation;

                    //we only need the operation ID, not the whole URL
                    const int numberOfCharsInOperationId = 36;
                    string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

                    //Get the ocr read results
                    ReadOperationResult results;
                    do
                    {
                        results = await client.GetReadResultAsync(Guid.Parse(operationId));
                    }
                    while ((results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted));

                    var fileResults = results.AnalyzeResult.ReadResults;
                    
                    var dates = from page in fileResults
                                         from line in page.Lines
                                         from word in line.Words
                                         where word?.Text?.Contains("EXP") ?? false
                                         select line.Words;

                    var expirationDate = dates.ToString();

                    PrintStatus($"Found Expiration Date {expirationDate} ");
                    //await Page.DisplayAlert("OCR Results", $"Found Expiration Date {expirationDate} ", null);

                    // 3. Add to data-bound collection
                    Coupons.Add(new Coupon
                    {
                        ExpirationDate = DateTime.Parse(expirationDate),
                        Photo = photo.Path,
                        TimeStamp = DateTime.Now
                    });
                    
                }
            }
            catch (Exception ex)
            {
                await (Application.Current?.MainPage?.DisplayAlert("Error",
                    $"Something bad happened: {ex.Message}", "OK") ??
                    Task.FromResult(true));

                PrintStatus(string.Format("ERROR: {0}", ex.Message));
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void PrintStatus(string helloWorld)
        {
            if (helloWorld == null)
                throw new ArgumentNullException(nameof(helloWorld));

            Debug.WriteLine(helloWorld);
        }
    }
}
