using System;
using System.Collections.Generic;
using UScheme;

namespace Logik.Core {
    public class SchemeEvaluator : IEvaluator {
        public string Type => "Scheme";

        Env env = new Env(Env.Global);

        public void Define(NumericCell cell) {
            var defString = $"(define ({cell.Name}) {cell.Formula})";
            var defExpression = Parser.Parse(defString);
            UScheme.UScheme.Eval(defExpression, env);
        }

        public void Undefine(NumericCell cell) {
            env.Remove(cell.Name);
        }

        public float Evaluate(NumericCell cell) {
            var formulaExp = Parser.Parse($"({cell.Name})");
            var ret = UScheme.UScheme.Eval(formulaExp, env);
            return float.Parse(ret.ToString());
        }

        public List<string> References(NumericCell cell) {
            var referenced = new List<string>();

            var formula = cell.Formula;
            foreach (var cellName in env.OwnNames()) {
                if (formula.Contains($"({cellName})")) {
                    referenced.Add(cellName);
                }
            }

            return referenced;
        }

        public void Rename(NumericCell cell, string newName) {
            throw new NotImplementedException();
        }
    }
}
