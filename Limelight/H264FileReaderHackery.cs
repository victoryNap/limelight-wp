﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;


namespace Limelight
{
    class H264FileReaderHackery
    {
        private StreamFrame frame;

        public H264FileReaderHackery(StreamFrame frame)
        {
            this.frame = frame;
        }


        public void readFile()
        {
            Debug.WriteLine("[H264FileReaderHackery::readFile]");
            var resourceStream  = App.GetResourceStream(new Uri("notpadded.h264", UriKind.Relative));
            var stream = resourceStream.Stream;
            Byte[] buffer = new Byte[131072];
            int offset = 0;
            int seekOffset = 0;
            while (stream.CanRead)
            {
                int len = stream.Read(buffer, offset, buffer.Length);
                if (len > 0) {
                    bool firstStart = false;
                    for (int i = 0; i < len - 4; i++) {
                        seekOffset++;
                        if (buffer[i] == 0 && buffer[i+1] == 0 && buffer[i+2] == 0 && buffer[i+3] == 1) {
                            if (firstStart) {
                                //we should decode the first i-1 bytes
                                //TODO: ^ that
                                IBuffer ms = buffer.AsBuffer(); 
                                frame.videoStream.TransportController_VideoMessageReceived(ms,0,10);
                                offset = --seekOffset;
                            } else {
                                firstStart = true;
                            }
                        } 
                    }
                } else {
                    Debug.WriteLine("[H264FileReaderHackery::readFile] No buffer");
                }
            }
        }
    }
}