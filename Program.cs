using System;
using System.Threading;
using Scarlet;
using Scarlet.Utilities;
using Scarlet.Communications;
using Scarlet.Controllers;
using Scarlet.IO;
using Scarlet.IO.BeagleBone;
using OpenTK.Input;

//0.0113 VOLTS/DEGREES

namespace UseRobot
{
    class MainClass
    {
        static bool ReceivingInput(GamePadState State)
        {
            return State.Triggers.Left <= Double.Epsilon && State.Triggers.Right <= Double.Epsilon;
        }

        const float P = 0.5f;
        public static void Main(string[] args)
        {
            Console.WriteLine("Initializing");
            StateStore.Start("Sasha and Jeremy");
            BeagleBone.Initialize(SystemMode.DEFAULT, true);
            BBBPinManager.AddMappingsI2C(BBBPin.P9_19, BBBPin.P9_20);
            BBBPinManager.AddMappingADC(BBBPin.P9_39);
            BBBPinManager.ApplyPinSettings(BBBPinManager.ApplicationMode.APPLY_IF_NONE);
            PID pid = new PID(P);
            IAnalogueIn Analog = new AnalogueInBBB(BBBPin.P9_39);
            AdafruitMotor[] Motor = new AdafruitMotor[4];
            for (int i = 0; i < 4; i++)
            {
                var pwm = new AdafruitMotorPWM((AdafruitMotorPWM.Motors)i, I2CBBB.I2CBus2, 0x60);
                Motor[i] = new AdafruitMotor(pwm);
            }
            GamePadState State;
            do
            {
                Console.WriteLine("Reading!");
                State = GamePad.GetState(0);
                if (State.IsConnected)
                    Console.WriteLine($"Left: {State.Triggers.Left}, Right: {State.Triggers.Right}");
                else
                    Console.WriteLine("NOT CONNECTED");
                double Val = 1.0;
                pid.Feed(Analog.GetInput());
                if (!ReceivingInput(State))
                {
                    Val = Analog.GetInput();
                }
                Motor[0].Speed = State.Triggers.Left * 75.0f + Val;
                Motor[0].UpdateState();
                Motor[1].Speed = State.Triggers.Right * 75.0f + Val;
                Motor[1].UpdateState();
                Motor[2].Speed = State.Triggers.Right * 75.0f + Val;
                Motor[2].UpdateState();
                Motor[3].Speed = State.Triggers.Left * 75.0f + Val;
                Motor[3].UpdateState();
            } while (State.Buttons.Start != ButtonState.Pressed);
        }
    }
}
