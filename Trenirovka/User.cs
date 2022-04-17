using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trenirovka
{
    /*16 и менее - выраженный дефицит
                                     * 16-18,5 - недостаточный
                                     * 18,5-25 - норма
                                     * 25-30 - избыточная масса тела
                                     * 30-35 - ожирение 1 степени
                                     * 35-40 - ожирение 2 степени                                    
                                     * 40 и более - ожирение 4 степени
                                     * 
                                     * 1 тип - от 16 до 30
                                     * 2 тип - от 30 до 40
                                     *  от 40 и более
                                     */
    public class User
    {
        public int ID_User { get; set; }
        public int Weight { get; set; }
        public int Rost { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public double Index { get; set; }
        public int TipeIndex { get; set; }
    }
}
