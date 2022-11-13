using System;
using NeuroWeb.EXMPL.INTERFACES;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public class NeuronActivate : IFunction {
        private IFunction.FunctionType _functionType;
        public void Set()
        {
            Console.WriteLine("Set actFunc pls\n1 - sigmoid \n2 - ReLU \n3 - th(x) \n");
            _functionType = Console.ReadLine() switch {
                "1" => IFunction.FunctionType.sigmoid,
                "2" => IFunction.FunctionType.ReLU,
                "3" => IFunction.FunctionType.thx,
                _ => _functionType
            };
        }
        
        public void Use(double[] value, int n)
        {
            switch (_functionType)
            {
                case IFunction.FunctionType.sigmoid:
                    for (var i = 0; i < n; i++)
                        value[i] = 1 / (1 + Math.Exp(-value[i]));
                    break;
                
                case IFunction.FunctionType.ReLU:
                    for (var i = 0; i < n; i++) {
                        value[i] = value[i] switch {
                            <0 => value[i] *= 0.01,
                            >1 => value[i] = 1d + 0.01 * (value[i] - 1d),
                            _ => 0
                        };
                    }
                    break;
                
                case IFunction.FunctionType.thx:
                    for (var i = 0; i < n; i++) {
                        if (value[i] < 0) 
                            value[i] = 0.01 * (Math.Exp(value[i]) - Math.Exp(-value[i])) / (Math.Exp(value[i]) + Math.Exp(-value[i]));
                        else 
                            value[i] = (Math.Exp(value[i]) - Math.Exp(-value[i])) / (Math.Exp(value[i]) + Math.Exp(-value[i]));
                    }
                    break;
                
                default:
                    throw new Exception();
            }
        }

        public void UseDer(double[] value, int n)
        {
            switch (_functionType)
            {
                case IFunction.FunctionType.sigmoid:
                    for (var i = 0; i < n; i++)
                        value[i] *= 1 - value[i];
                    break;
                
                case IFunction.FunctionType.ReLU:
                    for (var i = 0; i < n; i++) {
                        if (value[i] < 0 || value[i] > 1)
                            value[i] = 0.01;
                        else
                            value[i] = 1;
                    }
                    break;
                
                case IFunction.FunctionType.thx:
                    for (var i = 0; i < n; i++) {
                        if (value[i] < 0)
                            value[i] = 0.01 * (1 - value[i] * value[i]);
                        else
                            value[i] = 1 - value[i] * value[i];
                    }
                    break;
                
                default:
                    throw new Exception();
            }
        }

        public double UseDer(double value)
        {
            switch (_functionType)
            {
                case IFunction.FunctionType.sigmoid:
                    value = 1 / (1 + Math.Exp(-value));
                    break;
                
                case IFunction.FunctionType.ReLU:
                    if (value is < 0 or > 1)
                        value = 0.01;
                    break;
                
                case IFunction.FunctionType.thx:
                    if (value < 0) 
                        value = 0.01 * (Math.Exp(value) - Math.Exp(-value)) / (Math.Exp(value) + Math.Exp(-value));
                    else 
                        value = (Math.Exp(value) - Math.Exp(-value)) / (Math.Exp(value) + Math.Exp(-value));
                    break;
                
                default:
                    throw new Exception();
            }
            return value;
        }
    }
}