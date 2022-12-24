using System.Runtime;
using System.Security.Cryptography;

namespace Lomont.AdventOfCode._2019
{
    //2019 Day 12 part 1: 6849 in 14047.2 us
    //2019 Day 12 part 2: 356658899375688 in 30750497.4 us
    internal class Day12 : AdventOfCode
    {
        long GCD(long a, long b)
        {
            while (b != 0)
                (a, b) = (b, a % b);
            return a;

        }
        long LCM(long a, long b)
        {
            var d = GCD(a, b);
            return (a / d) * b;

        }
        public override object Run(bool part2)
        {
            // state
            var moons = new List<vec3>();
            var vel = new List<vec3>();
            var grav = new List<vec3>();

            foreach (var line in ReadLines())
            {
                var n  = Numbers(line);
                moons.Add(new vec3(n[0], n[1], n[2]));
                grav.Add(new vec3());
                vel.Add(new vec3());
            }

            // map key to seen step and period
            // tried looking at 3D periods (pos, vel, pos+vel, more), didn't work
            // separate into 1d problems since x,y,z independent :)
            // period of pos or vel alone didn't work, key on pos and vel, by coordinate
            var mx = new Dictionary<string, (long step,long period)>();
            var my = new Dictionary<string, (long step,long period)>();
            var mz = new Dictionary<string, (long step,long period)>();

            static void CheckPeriod(long step, string key, Dictionary<string,(long step, long period)> history, string msg, ref long save)
            {
                if (history.ContainsKey(key))
                {
                    var period = step - history[key].step;
                    //Console.WriteLine($"{msg} repeat at {step}, last {history[key]}, period {period}");
                    history[key] = (step,period);
                    save = period;
                }
                else
                    history.Add(key, (step,-1));
            }
            long perX = -1, perY = -1, perZ = -1;


            var steps = part2 ? int.MaxValue : 1000;
            for (var step = 0; step < steps; ++step)
            {
                if (part2)
                {
                    static string K(List<vec3> v1, List<vec3> v2, int dim) =>
                        v1.Aggregate(":", (a, b) => a + ',' + b[dim]) +
                        v2.Aggregate(":", (a, b) => a + ',' + b[dim]);

                    CheckPeriod(step, K(moons, vel, 0), mx, "moon x", ref perX);
                    CheckPeriod(step, K(moons, vel, 1), my, "moon y", ref perY);
                    CheckPeriod(step, K(moons, vel, 2), mz, "moon z", ref perZ);
                    if (perX != -1 && perY != -1 && perZ != -1)
                        return LCM(LCM(perX, perY), perZ);
                }

                for (var i = 0; i < moons.Count; i++)
                {
                    grav[i] = new vec3();
                    for (var j = 0; j < moons.Count; ++j)
                        grav[i] += (moons[j] - moons[i]).Sign(); // i==j ok here, don't care
                }
                // update
                for (var i = 0; i < moons.Count; i++)
                {
                    vel[i]   += grav[i];
                    moons[i] += vel[i];
                }
            }

            // energy for part1
            var energy = 0L;
            for (var i = 0; i < moons.Count; i++)
            {
                var pot = moons[i].ManhattanDistance;
                var kin = vel[i].ManhattanDistance;
                energy += (long)pot * kin;
            }

            return energy;

        }
    }
}