﻿using NUnit.Framework;

namespace Logik.Tests.Core {
    public class TestComparisonOperators : ParsingTest {
        [Test]
        public void TestLessThan() {
            WhenBuildingTree("4 < 5");
            ThenTreeEvalsTo(true);

            WhenBuildingTree("(17 * 2) < -99");
            ThenTreeEvalsTo(false);

        }

        [Test]
        public void TestLessOrEqualThan() {
            WhenBuildingTree("4 ≤ 5");
            ThenTreeEvalsTo(true);

            WhenBuildingTree("5 ≤ 5");
            ThenTreeEvalsTo(true);

            WhenBuildingTree("6 ≤ 5");
            ThenTreeEvalsTo(false);
        }

        [Test]
        public void TestGreaterThan() {
            WhenBuildingTree("4 > 5");
            ThenTreeEvalsTo(false);

            WhenBuildingTree("(17 * 2) > -99");
            ThenTreeEvalsTo(true);
        }

        [Test]
        public void TestGreaterOrEqualThan() {
            WhenBuildingTree("4 ≥ 5");
            ThenTreeEvalsTo(false);

            WhenBuildingTree("5 ≥ 5");
            ThenTreeEvalsTo(true);

            WhenBuildingTree("6 ≥ 5");
            ThenTreeEvalsTo(true);
        }
        
        [Test]
        public void TestEqual() {
            WhenBuildingTree("4 = 5");
            ThenTreeEvalsTo(false);

            WhenBuildingTree("5 = 5");
            ThenTreeEvalsTo(true);

            WhenBuildingTree("(6-1) = 5");
            ThenTreeEvalsTo(true);
        }
    }
}