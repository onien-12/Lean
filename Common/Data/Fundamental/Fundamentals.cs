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
*/

using System;
using QuantConnect.Data.UniverseSelection;

namespace QuantConnect.Data.Fundamental
{
    /// <summary>
    /// Lean fundamentals universe data class
    /// </summary>
    public class Fundamentals : BaseDataCollection
    {
        private static readonly Fundamental _factory = new();

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public Fundamentals()
        {
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="time">The current time</param>
        /// <param name="symbol">The associated symbol</param>
        public Fundamentals(DateTime time, Symbol symbol) : base(time, symbol)
        {
        }

        /// <summary>
        /// Return the URL string source of the file. This will be converted to a stream
        /// </summary>
        public override SubscriptionDataSource GetSource(SubscriptionDataConfig config, DateTime date, bool isLiveMode)
        {
            var path = _factory.GetSource(config, date, isLiveMode).Source;
            return new SubscriptionDataSource(path, SubscriptionTransportMedium.LocalFile, FileFormat.FoldingCollection);
        }

        /// <summary>
        /// Will read a new instance from the given line
        /// </summary>
        /// <param name="config">The associated requested configuration</param>
        /// <param name="line">The line to parse</param>
        /// <param name="date">The current time</param>
        /// <param name="isLiveMode">True if live mode</param>
        /// <returns>A new instance or null</returns>
        public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime date, bool isLiveMode)
        {
            try
            {
                var csv = line.Split(',');
                var symbol = new Symbol(SecurityIdentifier.Parse(csv[0]), csv[1]);
                return new Fundamental(date, symbol);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Will clone the current instance
        /// </summary>
        /// <returns>The cloned instance</returns>
        public override BaseData Clone()
        {
            return new Fundamentals(Time, Symbol) { Data = Data, EndTime = EndTime };
        }

        /// <summary>
        /// Creates the symbol used for coarse fundamental data
        /// </summary>
        /// <param name="market">The market</param>
        /// <returns>A coarse universe symbol for the specified market</returns>
        public static Symbol CreateUniverseSymbol(string market)
        {
            market = market.ToLowerInvariant();
            var ticker = $"qc-universe-fundamental-{market}-{Guid.NewGuid()}";
            var sid = SecurityIdentifier.GenerateEquity(SecurityIdentifier.DefaultDate, ticker, market);
            return new Symbol(sid, ticker);
        }
    }
}
