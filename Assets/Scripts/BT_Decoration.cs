using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Decoration
{
    public interface IDecoration
    {
        Node.Status Process();
        void Reset();
    }
}
