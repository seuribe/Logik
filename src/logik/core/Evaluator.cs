using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UScheme;

namespace Logik.Core {
    public class Evaluator {
        Env env = new Env(Env.Global);

        public void DefineCell(Cell cell, string formula) {
            var defString = $"(define ({cell.Id}) {formula})";
            var defExpression = UScheme.Parser.Parse(defString);
            UScheme.UScheme.Eval(defExpression, env);
        }

        public string EvaluateCell(Cell cell) {
            var callExpression = UScheme.Parser.Parse($"({cell.Id})");
            var ret = UScheme.UScheme.Eval(callExpression, env);
            return ret.ToString();
        }

        public List<string> GetReferencedCells(Cell cell) {
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
