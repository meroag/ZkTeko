using System;
using System.Collections.Generic;
using zkemkeeper;

namespace DataUpdater
{
    class FingerPrintDevice
    {
        readonly CZKEM device;
        private bool isConnected;
        private bool isZkBW;
        public FingerPrintDevice(bool isZkBW)
        {
            device = new CZKEM();
            this.isZkBW = isZkBW;
        }

        public bool Connect(string ip,int port)
        {
            isConnected = device.Connect_Net(ip,port);
            return isConnected;
        }

        public void Discoonect()
        {
            if (isConnected)
                device.Disconnect();
            isConnected = false;
        }

        public List<FingerPrintRecord> GetDataFromDevice()
        {
            if(!isConnected)
                return null;

            List<FingerPrintRecord> records = new List<FingerPrintRecord>();
            if (isZkBW)
            {
                int dwMachineNumber = 1;
                int dwTMachineNumber = 0;
                int dwEnrollNumber = 0;
                int dwEMachineNumber = 0;
                int dwVerifyMode = 0;
                int dwInOutMode = 0;
                int dwYear = 0;
                int dwMonth = 0;
                int dwDay = 0;
                int dwHour = 0;
                int dwMinute = 0;
                while (device.GetGeneralLogData(dwMachineNumber,ref dwTMachineNumber,ref dwEnrollNumber,ref dwEMachineNumber,ref dwVerifyMode,ref dwInOutMode,ref dwYear,ref dwMonth,ref dwDay,ref dwHour,ref dwMinute))
                {
                    FingerPrintRecord i = new FingerPrintRecord
                    {
                        IdUser = dwEnrollNumber,
                        Date = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, 0)
                    };

                    if (i.Date.ToShortDateString() == DateTime.Now.ToShortDateString())
                        records.Add(i);
                }
            }
            else
            {
                int dwMachineNumber = 1;
                string dwEnrollNumber;
                int dwVerifyMode;
                int dwInOutMode;
                int dwYear;
                int dwMonth;
                int dwDay;
                int dwHour;
                int dwMinute;
                int dwSecond;
                int dwWorkCode = 0;
                while (device.SSR_GetGeneralLogData(dwMachineNumber, out dwEnrollNumber, out dwVerifyMode,
                    out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond,
                    ref dwWorkCode))
                {
                    FingerPrintRecord i = new FingerPrintRecord
                    {
                        IdUser = Convert.ToInt32(dwEnrollNumber),
                        Date = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond)
                    };

                    if (i.Date.ToShortDateString() == DateTime.Now.ToShortDateString())
                        records.Add(i);
                }
            }

            return records;
        }
    }

    class FingerPrintRecord
    {
        public int IdUser { get; set; }
        public DateTime Date { get; set; }

        public override string ToString()
        {
            return "IdUser:" + IdUser + " Date:" + Date;
        }
    }
}
