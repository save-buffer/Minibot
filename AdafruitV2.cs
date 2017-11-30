using System;
using Scarlet.IO;
using Scarlet.IO.BeagleBone;

namespace UseRobot
{
    public class AdafruitV2
    {
        const byte PCA9685_MODE1 = 0x0;
        const byte PCA9685_PRESCALE = 0xFE;
        const byte LED0_ON_L = 0x6;

        readonly II2CBus i2c;
        byte address;
        byte i2caddr;
        ushort freq;
        public AdafruitV2(II2CBus the_i2c, byte addr = 0x60, byte i2caddr = 0x40)
        {
            this.i2c = the_i2c;
            address = addr;
            this.i2caddr = i2caddr;
        }

        public void Begin(ushort freq = 1600)
        {
            this.freq = freq;
            SetPwmFreq(freq);
            for (byte i = 0; i < 16; i++)
                SetPwm(i, 0, 0);

        }

        public void SetPwm(byte pin, ushort val)
        {
            if (val > 4095)
                SetPwm(pin, 4096, 0);
            else
                SetPwm(pin, 0, val);
        }

        public void SetPin(byte pin, bool value)
        {
            if (!value)
                SetPwm(pin, 0, 0);
            else
                SetPwm(pin, 4096, 0);
        }

        private void SetPwm(byte num, ushort on, ushort off)
        {
            i2c.Write(i2caddr, new byte[] { (byte)(LED0_ON_L + 4 * num),
            (byte)on,
            (byte)(on >> 8),
            (byte)(off),
            (byte)(off >> 8)
            });
        }

        private byte read8(byte addr)
        {
            return i2c.ReadRegister(i2caddr, addr, 1)[0];
        }

        private void write8(byte addr, byte d)
        {
            i2c.WriteRegister(i2caddr, addr, new byte[] { d });
        }

        private void SetPwmFreq(float frequ)
        {
            frequ *= 0.9f;
            float prescaleval = 25000000;
            prescaleval /= 4096;
            prescaleval /= frequ;
            prescaleval -= 1;

            byte prescale = (byte)Math.Floor(prescaleval + 0.5f);
            byte oldmode = read8(PCA9685_MODE1);
            byte newmode = (byte)((oldmode & 0x7F) | 0x10);
            write8(PCA9685_MODE1, newmode);
            write8(PCA9685_PRESCALE, prescale);
            write8(PCA9685_MODE1, oldmode);
            write8(PCA9685_MODE1, (byte)(oldmode | 0xa1));
        }
    }
}
