using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Utility.Classes;

namespace TacticsGame.Metrics
{
    using MetricTypeDictionary = Dictionary<string, Dictionary<string, double>>;
    using MetricDictionary = Dictionary<string, double>;

    using StatisticTypeDictionary = Dictionary<string, Dictionary<string, Statistic>>;
    using StatisticDictionary = Dictionary<string, Statistic>;

    using TimeStatTypeDictionary = Dictionary<string, Dictionary<string, Queue<Statistic>>>;
    using TimeStatDictionary = Dictionary<string, Queue<Statistic>>;

    [Serializable]
    public class MetricsLogger : Singleton<MetricsLogger>
    {
        private MetricTypeDictionary metrics = new MetricTypeDictionary();
        private StatisticTypeDictionary stats = new StatisticTypeDictionary();
        private TimeStatTypeDictionary timeStats = new TimeStatTypeDictionary();
        private MetricTypeDictionary metricsByUnit = new MetricTypeDictionary();

        private const int timeRange = 500;

        private MetricsLogger()
            : base()
        {
        }

        public StatisticTypeDictionary Stats { get { return stats; } }
        public MetricTypeDictionary Metrics { get { return metrics; } }
        public MetricTypeDictionary MetricsByUnit { get { return metricsByUnit; } }
        public TimeStatTypeDictionary TimeStats { get { return timeStats; } }

        public MetricDictionary GetMetricByType(MetricType type, bool addIfNotExists = true)
        {
            return this.GetMetricByType(type.ToString(), addIfNotExists);
        }

        /// <summary>
        /// Gets the metric by dictionary. If it does not exist, adds creates a new one, if the arg is true.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MetricDictionary GetMetricByType(string type, bool addIfNotExists = true) 
        {
            if (!Instance.Metrics.ContainsKey(type)) 
            {
                if (addIfNotExists)
                {
                    Instance.Metrics[type] = new MetricDictionary();
                }
                else
                {
                    return null;
                }
            }            

            return Instance.Metrics[type];
        }

        /// <summary>
        /// Gets the metric by dictionary. If it does not exist, adds creates a new one, if the arg is true.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MetricDictionary GetUnitMetricByType(string key, bool addIfNotExists = true)
        {
            if (!Instance.MetricsByUnit.ContainsKey(key))
            {
                if (addIfNotExists)
                {
                    Instance.MetricsByUnit[key] = new MetricDictionary();
                }
                else
                {
                    return null;
                }
            }

            return Instance.MetricsByUnit[key];
        }

        public StatisticDictionary GetStatisticByType(MetricType type, bool addIfNotExists = true)
        {
            return this.GetStatisticByType(type.ToString(), addIfNotExists);
        }

        /// <summary>
        /// Gets the metric by dictionary. If it does not exist, adds creates a new one, if the arg is true.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public StatisticDictionary GetStatisticByType(string type, bool addIfNotExists = true)
        {
            
            if (!Instance.Stats.ContainsKey(type))
            {
                if (addIfNotExists) 
                {
                    Instance.Stats[type] = new StatisticDictionary();
                }
                else
                {
                    return null;
                }
            }            

            return Instance.Stats[type];
        }

        private TimeStatDictionary GetTimeStatsByType(MetricType type)
        {
            return this.GetTimeStatsByType(type.ToString());
        }

        private TimeStatDictionary GetTimeStatsByType(string type)
        {
            return Instance.TimeStats.ContainsKey(type) ? Instance.TimeStats[type] : (Instance.TimeStats[type] = new TimeStatDictionary());
        }

        public static double GetMetric(MetricType type, string datum)
        {
            MetricDictionary dictionary = Instance.GetMetricByType(type);
            return dictionary.ContainsKey(datum) ? dictionary[datum] : 0.0d;
        }

        public static void LogAppendStatistic(MetricType type, string datum, int value)
        {
            //LogAppendStatistic(type.ToString(), datum, value);
        }

        public static void LogAppendStatistic(string type, string datum, int value)
        {
            //StatisticDictionary dictionary = Instance.GetStatisticByType(type);

            //Statistic current = dictionary.ContainsKey(datum) ? dictionary[datum] : new Statistic();
            //dictionary[datum] = current + value;

            //TimeStatDictionary timeDictionary = Instance.GetTimeStatsByType(type);
            //Queue<Statistic> timeLine = timeDictionary.ContainsKey(datum) ? timeDictionary[datum] : (timeDictionary[datum] = new Queue<Statistic>());
            //timeLine.Enqueue(current + value);
            //if (timeLine.Count > timeRange)
            //{
            //    timeLine.Dequeue();
            //}
        }

        public static void LogAppend(MetricType type, string datum, double value)
        {
            //LogAppend(type.ToString(), datum, value);
        }

        public static void LogAppend(string type, string datum, double value)
        {
            //MetricDictionary dictionary = Instance.GetMetricByType(type);

            //double current = dictionary.ContainsKey(datum) ? dictionary[datum] : 0.0d;
            //dictionary[datum] = current + value;
        }

        public static void LogAppendByUnit(string type, string unit, string datum, double value)
        {
            //MetricDictionary dictionary = Instance.GetUnitMetricByType(type + "_" + unit);

            //double current = dictionary.ContainsKey(datum) ? dictionary[datum] : 0.0d;
            //dictionary[datum] = current + value;
        }

        public static void Log(MetricType type, string datum, double value)
        {
            //Log(type.ToString(), datum, value);
        }

        public static void Log(string type, string datum, double value)
        {
            //MetricDictionary dictionary = Instance.GetMetricByType(type);
            //dictionary[datum] = value;
        }

        public void ClearAll()
        {
            this.metrics.Clear();
            this.metricsByUnit.Clear();
            this.stats.Clear();
            this.timeStats.Clear();
        }
    }

    public struct Statistic 
    {
        public long TotalSum;
        public int N;
        
        private Statistic(long totalSum, int n)
        {
            this.TotalSum = totalSum;
            this.N = n;
        }

        public int Average { get { return (int)(TotalSum / (long)N); } }        

        public static Statistic operator + (Statistic one, int other) 
        {
            return new Statistic(one.TotalSum + other, one.N + 1);
        }
    }

    public enum MetricType
    {
        Decisions,
        BuildingToBuyFrom,
        Propensity,
        BuyingBid,
        SellingBid,
        AppraisalInaccuracy,
        ReassessmentImprovement,
        UnitWantsToBuy,
        ReassessmentBail,
        ReassessmentRetry,
        WillingnessToSell,
        WantsToBuyFrom,
        WantToSellTo,
        ReassessmentPercentChange,
        BuildingToSellTo,
        VisitorToSellTo,
        WillingnessToBuy_Units,
        WillingnessToBuy_Items,
        ItemsBought,
        ItemsSold,
    }
}
