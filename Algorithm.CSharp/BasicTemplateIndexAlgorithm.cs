/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

using System;
using QuantConnect.Data;
using System.Collections.Generic;
using QuantConnect.Indicators;
using QuantConnect.Interfaces;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// This example demonstrates how to add index asset types.
    /// </summary>
    /// <meta name="tag" content="using data" />
    /// <meta name="tag" content="benchmarks" />
    /// <meta name="tag" content="indexes" />
    public class BasicTemplateIndexAlgorithm : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        protected Symbol Spx;
        protected Symbol SpxOption;
        private ExponentialMovingAverage _emaSlow;
        private ExponentialMovingAverage _emaFast;

        protected virtual Resolution Resolution => Resolution.Minute;
        protected virtual int StartDay => 4;

        /// <summary>
        /// Initialize your algorithm and add desired assets.
        /// </summary>
        public override void Initialize()
        {
            SetStartDate(2021, 1, StartDay);
            SetEndDate(2021, 1, 18);
            SetCash(1000000);

            // Use indicator for signal; but it cannot be traded
            Spx = AddIndex("SPX", Resolution).Symbol;

            // Trade on SPX ITM calls
            SpxOption = QuantConnect.Symbol.CreateOption(
                Spx,
                Market.USA,
                OptionStyle.European,
                OptionRight.Call,
                3200m,
                new DateTime(2021, 1, 15));

            AddIndexOptionContract(SpxOption, Resolution);

            _emaSlow = EMA(Spx, 80);
            _emaFast = EMA(Spx, 200);
        }

        /// <summary>
        /// Index EMA Cross trading underlying.
        /// </summary>
        public override void OnData(Slice slice)
        {
            if (!slice.Bars.ContainsKey(Spx) || !slice.Bars.ContainsKey(SpxOption))
            {
                return;
            }

            // Warm up indicators
            if (!_emaSlow.IsReady)
            {
                return;
            }

            if (_emaFast > _emaSlow)
            {
                SetHoldings(SpxOption, 1);
            }
            else
            {
                Liquidate();
            }
        }

        public override void OnEndOfAlgorithm()
        {
            if (Portfolio[Spx].TotalSaleVolume > 0)
            {
                throw new Exception("Index is not tradable.");
            }
        }

        /// <summary>
        /// This is used by the regression test system to indicate if the open source Lean repository has the required data to run this algorithm.
        /// </summary>
        public virtual bool CanRunLocally { get; } = true;

        /// <summary>
        /// This is used by the regression test system to indicate which languages this algorithm is written in.
        /// </summary>
        public virtual Language[] Languages { get; } = { Language.CSharp, Language.Python };

        /// <summary>
        /// Data Points count of all timeslices of algorithm
        /// </summary>
        public virtual long DataPoints => 15929;

        /// <summary>
        /// Data Points count of the algorithm history
        /// </summary>
        public virtual int AlgorithmHistoryDataPoints => 0;

        /// <summary>
        /// This is used by the regression test system to indicate what the expected statistics are from running the algorithm
        /// </summary>
        public virtual Dictionary<string, string> ExpectedStatistics => new Dictionary<string, string>
        {
            {"Total Trades", "3"},
            {"Average Win", "0%"},
            {"Average Loss", "-7.01%"},
            {"Compounding Annual Return", "-93.456%"},
            {"Drawdown", "9.300%"},
            {"Expectancy", "-1"},
            {"Net Profit", "-8.354%"},
            {"Sharpe Ratio", "-4.078"},
            {"Probabilistic Sharpe Ratio", "0.781%"},
            {"Loss Rate", "100%"},
            {"Win Rate", "0%"},
            {"Profit-Loss Ratio", "0"},
            {"Alpha", "-0.377"},
            {"Beta", "0.178"},
            {"Annual Standard Deviation", "0.069"},
            {"Annual Variance", "0.005"},
            {"Information Ratio", "-7.596"},
            {"Tracking Error", "0.108"},
            {"Treynor Ratio", "-1.583"},
            {"Total Fees", "$0.00"},
            {"Estimated Strategy Capacity", "$9100000.00"},
            {"Lowest Capacity Asset", "SPX XL80P3GHDZXQ|SPX 31"},
            {"Fitness Score", "0.051"},
            {"Kelly Criterion Estimate", "0"},
            {"Kelly Criterion Probability Value", "0"},
            {"Sortino Ratio", "-1.51"},
            {"Return Over Maximum Drawdown", "-11.301"},
            {"Portfolio Turnover", "0.338"},
            {"Total Insights Generated", "0"},
            {"Total Insights Closed", "0"},
            {"Total Insights Analysis Completed", "0"},
            {"Long Insight Count", "0"},
            {"Short Insight Count", "0"},
            {"Long/Short Ratio", "100%"},
            {"Estimated Monthly Alpha Value", "$0"},
            {"Total Accumulated Estimated Alpha Value", "$0"},
            {"Mean Population Estimated Insight Value", "$0"},
            {"Mean Population Direction", "0%"},
            {"Mean Population Magnitude", "0%"},
            {"Rolling Averaged Population Direction", "0%"},
            {"Rolling Averaged Population Magnitude", "0%"},
            {"OrderListHash", "7f5f25fafc99bf45774e63bc528f3fc8"}
        };
    }
}
