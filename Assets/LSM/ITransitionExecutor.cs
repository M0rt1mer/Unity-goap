using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ITransitionExecutor{

    void ExecuteTransition(IStateMachineTransition transition);

    }
