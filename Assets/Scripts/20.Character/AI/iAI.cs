using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iAI {
    bool Check(AIHolder aiHolder);
    void Do();
}