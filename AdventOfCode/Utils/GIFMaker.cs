﻿using Lomont.GIFLib;

namespace Lomont.AdventOfCode.Utils
{
    internal class GIFMaker
    {
        int w, h;
        SimpleGif gif;
        byte[] palette = new byte[768];

        public GIFMaker(int w, int h)
        {
            this.w = w;
            this.h  =h;
            AtariPalette(palette);

            gif = new SimpleGif(w, h, bitsPerPixel: 8, globalPalette: palette);
        }

        // data is w,h
        public void AddFrame(byte[,] data, int delay100thOfSecond)
        {
            var data2 = new byte[w * h];
            for (var j = 0; j < h; ++j)
            for (var i = 0; i < w; ++i)
                data2[i + j * w] = data[i, j];
            gif.AddFrame(frameData: data2, delayHundredthsOfSecond: delay100thOfSecond);
        }

        public void Save(string filename)
        {
            gif.WriteImage(filename);
        }

        void AtariPalette(byte[] palette)
        { // https://lospec.com/palette-list/atari-8-bit-family-gtia
            var st = """
#000000
#111111
#222222
#333333
#444444
#555555
#666666
#777777
#888888
#999999
#aaaaaa
#bbbbbb
#cccccc
#dddddd
#eeeeee
#ffffff
#190700
#2a1800
#3b2900
#4c3a00
#5d4b00
#6e5c00
#7f6d00
#907e09
#a18f1a
#b3a02b
#c3b13c
#d4c24d
#e5d35e
#f7e46f
#fff582
#ffff96
#310000
#3f0000
#531700
#642800
#753900
#864a00
#975b0a
#a86c1b
#b97d2c
#ca8e3d
#db9f4e
#ecb05f
#fdc170
#ffd285
#ffe39c
#fff4b2
#420404
#4f0000
#600800
#711900
#822a0d
#933b1e
#a44c2f
#b55d40
#c66e51
#d77f62
#e89073
#f9a183
#ffb298
#ffc3ae
#ffd4c4
#ffe5da
#410103
#50000f
#61001b
#720f2b
#83203c
#94314d
#a5425e
#b6536f
#c76480
#d87591
#e986a2
#fa97b3
#ffa8c8
#ffb9de
#ffcaef
#fbdcf6
#330035
#440041
#55004c
#660c5c
#771d6d
#882e7e
#993f8f
#aa50a0
#bb61b1
#cc72c2
#dd83d3
#ee94e4
#ffa5e4
#ffb6e9
#ffc7ee
#ffd8f3
#1d005c
#2e0068
#400074
#511084
#622195
#7332a6
#8443b7
#9554c8
#a665d9
#b776ea
#c887eb
#d998eb
#e9a9ec
#fbbaeb
#ffcbef
#ffdff9
#020071
#13007d
#240b8c
#351c9d
#462dae
#573ebf
#684fd0
#7960e1
#8a71f2
#9b82f7
#ac93f7
#bda4f7
#ceb5f7
#dfc6f7
#f0d7f7
#ffe8f8
#000068
#000a7c
#081b90
#192ca1
#2a3db2
#3b4ec3
#4c5fd4
#5d70e5
#6e81f6
#7f92ff
#90a3ff
#a1b4ff
#b2c5ff
#c3d6ff
#d4e7ff
#e5f8ff
#000a4d
#001b63
#002c79
#023d8f
#134ea0
#245fb1
#3570c2
#4681d3
#5792e4
#68a3f5
#79b4ff
#8ac5ff
#9bd6ff
#ace7ff
#bdf8ff
#ceffff
#001a26
#002b3c
#003c52
#004d68
#065e7c
#176f8d
#28809e
#3991af
#4aa2c0
#5bb3d1
#6cc4e2
#7dd5f3
#8ee6ff
#9ff7ff
#b0ffff
#c1ffff
#01250a
#023610
#004622
#005738
#05684d
#16795e
#278a6f
#389b80
#49ac91
#5abda2
#6bceb3
#7cdfc4
#8df0d5
#9effe5
#affff1
#c0fffd
#04260d
#043811
#054713
#005a1b
#106b1b
#217c2c
#328d3d
#439e4e
#54af5f
#65c070
#76d181
#87e292
#98f3a3
#a9ffb3
#baffbf
#cbffcb
#00230a
#003510
#044613
#155613
#266713
#377813
#488914
#599a25
#6aab36
#7bbc47
#8ccd58
#9dde69
#aeef7a
#bfff8b
#d0ff97
#e1ffa3
#001707
#0e2808
#1f3908
#304a08
#415b08
#526c08
#637d08
#748e0d
#859f1e
#96b02f
#a7c140
#b8d251
#c9e362
#daf473
#ebff82
#fcff8e
#1b0701
#2c1801
#3c2900
#4d3b00
#5f4c00
#705e00
#816f00
#938009
#a4921a
#b2a02b
#c7b43d
#d8c64e
#ead760
#f6e46f
#fffa84
#ffff99
""";

            var cols = st.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Trace.Assert(cols.Length == 256);
            int i = 0;
            foreach (var s1 in cols)
            {
                var s = s1.Trim().ToLower();
                var m = Regex.Match(s, @"#[0-9a-f]{6}");
                Trace.Assert(m.Success);

                var c = m.Value;
                palette[i] = Hex(c[1..3]);
                palette[i + 1] = Hex(c[3..5]);
                palette[i + 2] = Hex(c[5..7]);
                i += 3;
            }

            byte Hex(string cs)
            {
                var hex = "0123456789abcdef";
                var i1 = hex.IndexOf(cs[0]);
                var i2 = hex.IndexOf(cs[1]);
                Trace.Assert(0 <= i1 && 0 <= i2);
                return (byte)(i1 * 16 + i2);
            }

        }

    }
}
