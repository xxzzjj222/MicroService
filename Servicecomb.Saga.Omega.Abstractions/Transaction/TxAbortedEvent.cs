﻿/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


namespace Servicecomb.Saga.Omega.Abstractions.Transaction
{
    public class TxAbortedEvent : TxEvent
    {
        private const int PayloadsMaxLength = 10240;

        public TxAbortedEvent(string globalTxId, string localTxId, string parentTxId, string compensationMethod, System.Exception throwable) : base(EventType.TxAbortedEvent, globalTxId, localTxId, parentTxId, compensationMethod, 0, "", 0,
            StackTrace(throwable))
        {
        }

        private static string StackTrace(System.Exception throwable)
        {
            if (throwable.StackTrace.Length > PayloadsMaxLength)
            {
                return throwable.StackTrace.Substring(PayloadsMaxLength);
            }

            return throwable.StackTrace;
        }
    }
}
