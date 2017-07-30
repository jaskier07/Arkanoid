using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    //Blok, który zawiera dane zwykłego bloku. Jego celem jest serializacja i deserializacja jego danych
    //w celu późniejszego wczytania zapisanego etapu.
    public class SerializableBlock
    {
        public int blockType { get; set; }
        public int gridx { get; set; }
        public int gridy { get; set; }

        public SerializableBlock() { }

        public SerializableBlock(int blockType, int gridx, int gridy)
        {
            this.blockType = blockType;
            this.gridx = gridx;
            this.gridy = gridy;
        }
    }
}
