using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOLInstructionSet : MonoBehaviour
{
    //use this for initilialization
    
    public int[] _instructions = new int[4];

        public GOLInstructionSet (int[] instructions)
    {
       _instructions = instructions;
     }

    public GOLInstructionSet(int inst0, int inst1, int inst2, int inst3)
    {
        _instructions[inst0] = inst0;
        _instructions[inst1] = inst1;
        _instructions[inst2] = inst2;
        _instructions[inst3] = inst3;
     
    }
        public int getInstruction(int _index)
    {
        return _instructions[_index];
    }

}
