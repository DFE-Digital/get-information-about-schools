using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Common
{
    public static class Retry
    {
        /// <summary>
        /// Represents an action that may fail numerous times, but ultimately will succeed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="minInterval">Interval in milliseconds</param>
        /// <param name="maxInterval">Interval in milliseconds</param>
        /// <param name="maxRetries">If zero, try forever</param>
        /// <returns></returns>
        public static T RetryableAction<T>(Func<T> func, int minInterval = 100, int maxInterval = 500, int maxRetries = 5,
            Action<Exception> failureAction = null, Func<T, bool> failureCondition = null)
        {
            int count = 0;
            var rnd = new Random();
            while (true)
            {
                count++;
                try
                {
                    T retVal = func();
                    if (failureCondition == null) return retVal;
                    else if (failureCondition(retVal)) throw new ApplicationException("Failed condition check");
                    else if (!failureCondition(retVal)) return retVal;
                }
                catch (Exception ex)
                {
                    failureAction?.Invoke(ex);
                    if (count > maxRetries && maxRetries > 0) throw;
                    System.Threading.Thread.Sleep(rnd.Next(minInterval, maxInterval));
                }
            }

        }

        /// <summary>
        /// Retries performing an action according to the supplied constraints
        /// </summary>
        /// <param name="func"></param>
        /// <param name="minInterval"></param>
        /// <param name="maxInterval"></param>
        /// <param name="maxRetries"></param>
        /// <param name="failureAction"></param>
        public static void RetryableAction(Action func,
            int minInterval = 100,
            int maxInterval = 500,
            int maxRetries = 5,
            Action<Exception> failureAction = null)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try { func(); return; }
                catch (Exception ex)
                {
                    failureAction?.Invoke(ex);
                    if (i == (maxRetries - 1)) throw;
                    else System.Threading.Thread.Sleep(RandomNumber.Next(minInterval, maxInterval));
                }
            }
        }



        /// <summary>
        /// Retries performing an action according to the supplied constraints
        /// </summary>
        /// <param name="func"></param>
        /// <param name="minInterval"></param>
        /// <param name="maxInterval"></param>
        /// <param name="maxRetries"></param>
        /// <param name="failureAction"></param>
        public static async Task RetryableActionAsync(Func<Task> func,
            int minInterval = 100,
            int maxInterval = 500,
            int maxRetries = 5,
            Action<Exception> failureAction = null)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await func();
                    break;
                }
                catch (Exception ex)
                {
                    failureAction?.Invoke(ex);
                    if (i == (maxRetries - 1)) throw;
                    else await Task.Delay(RandomNumber.Next(minInterval, maxInterval));
                }
            }
        }





    }
}
