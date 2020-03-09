using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iAI {
    void Check(GameObject who);
    void Do(GameObject who);
}