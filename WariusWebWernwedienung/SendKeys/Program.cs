var parameter = string.Join(" ", args);
parameter = parameter.Replace('_', ' ');
SendKeys.SendWait(parameter);
