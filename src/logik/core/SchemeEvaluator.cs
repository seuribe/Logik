using System;
using System.Collections.Generic;
using UScheme;

namespace Logik.Core {
    public class SchemeEvaluator : IEvaluator {
        public string Type => "Scheme";

        Env env = new Env(Env.Global);

        public void Define(Cell cell) {
            var defString = $"(define ({cell.Name}) {cell.Formula})";
            var defExpression = Parser.Parse(defString);
            UScheme.UScheme.Eval(defExpression, env);
        }

        public void Undefine(Cell cell) {
            env.Remove(cell.Name);
        }

        public string Evaluate(Cell cell) {
            var formulaExp = Parser.Parse($"({cell.Name})");
            var ret = UScheme.UScheme.Eval(formulaExp, env);
            return ret.ToString();
        }

        public List<string> References(Cell cell) {
            var referenced = new List<string>();

            var formula = cell.Formula;
            foreach (var cellName in env.OwnNames()) {
                if (formula.Contains($"({cellName})")) {
                    referenced.Add(cellName);
                }
            }

            return referenced;
        }

        public void Rename(Cell cell, string newName) {
            throw new NotImplementedException();
        }
    }
}
