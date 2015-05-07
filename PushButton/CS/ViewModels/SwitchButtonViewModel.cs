using PushButton.Commands;
using System;
using System.Windows.Input;
using Windows.Devices.Gpio;

namespace PushButton.ViewModels
{
    public class SwitchButtonViewModel : ViewModelBase, IDisposable
    {
        private const int LED_PIN = 27;
        private const int PB_PIN = 5;

        private GpioPin _ledPin;
        private GpioPin _pushButton;
        private DateTime _lastEvent;
        private TimeSpan _timeOut = TimeSpan.FromSeconds(0.25);

        private bool _ledIsOn;
        public bool LEDIsOn
        {
            get { return _ledIsOn; }
            set
            {
                if (_ledIsOn == value)
                    return;

                _ledIsOn = value;
                OnPropertyChanged();
            }
        }

        private string _gpioStatusText = "Waiting for GPIO Pin to initialize ...";
        public string GPIOStatusText
        {
            get { return _gpioStatusText; }
            set
            {
                if (_gpioStatusText == value)
                    return;

                _gpioStatusText = value;
                OnPropertyChanged();
            }
        }

        public ICommand RequestFlipLED { get; set; }

        public SwitchButtonViewModel()
        {
            InitGPIO();
            RequestFlipLED = new RelayCommand((o) => FlipLED());
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                _ledPin = null;
                GPIOStatusText = "There is no GPIO controller on this device.";
                return;
            }

            _pushButton = gpio.OpenPin(PB_PIN);
            _ledPin = gpio.OpenPin(LED_PIN);

            // Show an error if the pin wasn't initialized properly
            if (_ledPin == null)
            {
                GPIOStatusText= "There were problems initializing the GPIO LED pin.";
                return;
            }
            if (_pushButton == null)
            {
                GPIOStatusText = "There were problems initializing the GPIO Push Button pin.";
                return;
            }

            _pushButton.SetDriveMode(GpioPinDriveMode.Input);
            _pushButton.ValueChanged += _pushButton_ValueChanged;
            _ledPin.SetDriveMode(GpioPinDriveMode.Output);

            GPIOStatusText = "GPIO pin initialized correctly.";
        }

        private void _pushButton_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (!(args.Edge == GpioPinEdge.RisingEdge))
                return;

            if (!(_lastEvent.Add(_timeOut) < DateTime.Now))
                return;

            _lastEvent = DateTime.Now;

            FlipLED();
        }

        private void FlipLED()
        {
            if (LEDIsOn)
            {
                _ledPin.Write(GpioPinValue.High);
            }
            else
            {
                _ledPin.Write(GpioPinValue.Low);
            }

            LEDIsOn = !LEDIsOn;
        }

        ~SwitchButtonViewModel()
        {
            Dispose();
        }
        public void Dispose()
        {
            _ledPin.Dispose();
            _pushButton.Dispose();
        }
    }
}
