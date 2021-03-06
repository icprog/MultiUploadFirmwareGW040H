﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiUploadFirmwareGW040H.Function.Protocol {
    public class Serial {

        SerialPort _serialport = null;
        string _PortName = "";

        /// <summary>
        /// CONSTRUCTOR KHỞI TẠO CLASS
        /// </summary>
        /// <param name="_portname"></param>
        public Serial(string _portname) {
            this._PortName = _portname;
        }

        /// <summary>
        /// MỞ KẾT NỐI CỔNG COM PORT / RETRY 3 LẦN
        /// </summary>
        /// <returns></returns>
        public bool Open(out string _message) {
            _message = "";
            int count = 0;
            bool result = false;
            REP:
            count++;
            try {
                this._serialport = new SerialPort();
                this._serialport.PortName = _PortName;
                this._serialport.BaudRate = 115200;
                this._serialport.DataBits = 8;
                this._serialport.Parity = Parity.None;
                this._serialport.StopBits = StopBits.One;
                this._serialport.Open();
                //_serialport.DataReceived += new SerialDataReceivedEventHandler(serial_OnReceiveData);
                result = _serialport.IsOpen;
            }
            catch (Exception ex) {
                _message = ex.ToString();
                result = false;
            }
            if (!result) { if (count < 3) { Thread.Sleep(100); goto REP; } }
            return result;
        }

        /// <summary>
        /// ĐÓNG KẾT NỐI CỔNG COM
        /// </summary>
        /// <returns></returns>
        public bool Close() {
            try {
                this._serialport.Close();
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// GHI DỮ LIỆU VÀO CỔNG SERIALPORT
        /// </summary>
        /// <param name="_cmd"></param>
        /// <returns></returns>
        public bool Write(string _cmd) {
            try {
                this._serialport.Write(_cmd);
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// GHI DỮ LIỆU VÀO CỔNG SERIALPORT
        /// </summary>
        /// <param name="_cmd"></param>
        /// <returns></returns>
        public bool WriteLine(string _cmd) {
            try {
                this._serialport.WriteLine(_cmd);
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cmd"></param>
        /// <returns></returns>
        public bool WriteLineAndWaitComplete(string _cmd, ref string msg) {
            int counter = 0;
            bool result = false;
            REP:
            counter++;
            try {
                this._serialport.ReadExisting();
                this._serialport.WriteLine(_cmd);
                bool _flag = false;
                string _data = "";
                int _count = 0;

                while (_flag == false) {
                    Thread.Sleep(100);
                    _data += this._serialport.ReadExisting();
                    if (_data.Contains("#")) _flag = true;
                    _count++;
                    if (_count >= 50) break;
                }
                msg = _data;
                result = _flag;
            }
            catch (Exception ex) {
                msg = ex.ToString();
                result = false;
            }
            //-------------------------------------//
            if (result == false && counter < 3) goto REP;
            else return result;
        }

        /// <summary>
        /// ĐỌC DỮ LIỆU TỪ CỔNG SERIALPORT
        /// </summary>
        /// <returns></returns>
        public string Read() {
            try {
                return this._serialport.ReadExisting();
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString(), "ERROR", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return "";
            }
        }

    }
}
