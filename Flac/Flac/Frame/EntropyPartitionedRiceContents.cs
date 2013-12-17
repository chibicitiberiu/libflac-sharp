using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flac.Frame
{
    public class EntropyPartitionedRiceContents
    {
        public int[] Parameters { get; set; }
        public int[] RawBits { get; set; }

        /// <summary>
        /// The capacity of the parameters and raw_bits arrays specified as an order.
        /// i.e. the number of array elements allocated is 2 ^ capacity_by_order.
        /// </summary>
        private int capacityByOrder = 0;

        /// <summary>
        /// Ensure enough menory has been allocated.
        /// </summary>
        /// <param name="maxPartitionOrder">The maximum partition order</param>
        public void EnsureSize(int maxPartitionOrder)
        {
            if (capacityByOrder >= maxPartitionOrder)
                return;

            Parameters = new int[(1 << maxPartitionOrder)];
            RawBits = new int[(1 << maxPartitionOrder)];

            capacityByOrder = maxPartitionOrder;
        }
    }
}
