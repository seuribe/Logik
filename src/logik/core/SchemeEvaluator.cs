using System;
using System.Collections.Generic;
using UScheme;

namespace Logik.Core {
    public class SchemeEvaluator {
        Env env = new Env(Env.Global);
        Env formulas = new Env(Env.Global);

        public void DefineCell(Cell cell) {
            var defString = $"(define {cell.Id} {cell.Value})";
            var defExpression = Parser.Parse(defString);
            UScheme.UScheme.Eval(defExpression, env);

            defString = $"(define ({cell.Id}) {cell.Formula})";
            defExpression = Parser.Parse(defString);
            UScheme.UScheme.Eval(defExpression, formulas);
        }

        public void UndefineCell(Cell cell) {
            env.Remove(cell.Id);
            formulas.Remove(cell.Id);
        }

        public string EvaluateCell(Cell cell) {
            var formulaExp = formulas.Get(cell.Id);
            var ret = UScheme.UScheme.Eval(formulaExp, env);
            return ret.ToString();
        }

        public List<string> GetReferencedIds(Cell cell) {
            var referenced = new List<string>();

            var formula = cell.Formula;
            foreach (var cellName in env.OwnNames()) {
                if (formula.Contains($"({cellName})")) {
                    referenced.Add(cellName);
                }
            }

            return referenced;
        }
    }
}
