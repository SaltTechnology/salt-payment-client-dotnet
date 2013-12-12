using System.Net;
using System.Text;
using System;
using System.IO;

namespace com.admeris.creditcard.api{
    public class PeriodicPurchaseInfo {
         /// ScheduleType = monthly, weekly, daily etc 
		public enum ScheduleType { 
			NULL = -1,
			MONTH = 0,
			WEEK = 1,
			DAY = 2
		};
		
		public class State{

			public static readonly State NULL = new State(-1);
			public static readonly State NEW = new State(0);
			public static readonly State IN_PROGRESS = new State(1);
			public static readonly State COMPLETE = new State(2);
			public static readonly State ON_HOLD = new State(3);
			public static readonly State CANCELLED = new State(4);

			private readonly short code;
			private string type;

			State(int code){				
				this.code = (short) code;
			}
			
			public static State fromCode(short code){
				State currentState = new State(code);
				if ((-1 <= code)&& (code <= 4)){
					return currentState;
				} 
				else {
					throw new ArgumentException("the code [%code] does not correspond to any State", "code");
				}
			}	
			
			public short toCode(){
				return this.code;
			}
			
			public override string ToString(){
				if (this.code == -1)
					this.type = "NULL";
				else if (this.code == 0)
					this.type = "NEW";
				else if (this.code == 1)
					this.type = "IN_PROGRESS";
				else if (this.code ==2)
					this.type = "COMPLETE";
				else if (this.code == 3)
					this.type = "ON_HOLD";
				else if (this.code == 4)
					this.type= "CANCELLED";
				else
					this.type = "UNKNOWN";
				return this.type;
			}
			
		}
		
		///<summary>Defines a recurring schedule</summary>
		public class Schedule {
			private ScheduleType scheduleType;
			private short intervalLength;

			public Schedule(ScheduleType type, short intervalLength) {
				this.scheduleType = type;
				this.intervalLength = intervalLength;
			}

			public ScheduleType getScheduleType() {
				return this.scheduleType;
			}
			public short getIntervalLength() {
				return this.intervalLength;
			}
		}

		private long periodicTransactionId;
		private long lastPaymentId;
		private State state;
		private Schedule schedule;
		private long perPaymentAmount;
		private string orderId;
		private string customerId;
		private DateTime startDate;
		private DateTime endDate;
		private DateTime nextPaymentDate;
		
		public PeriodicPurchaseInfo(long periodicTransactionId, State state) {
			this.periodicTransactionId = periodicTransactionId;
			this.state = state;
		}
		
		public PeriodicPurchaseInfo(long periodicTransactionId, State state, DateTime nextPaymentDate, long lastPaymentId){
			this.periodicTransactionId = periodicTransactionId;
			this.state = state;
			this.nextPaymentDate = nextPaymentDate;
			this.lastPaymentId = lastPaymentId;
		}
		public PeriodicPurchaseInfo(long periodicTransactionId, State state, long perPaymentAmount) {
			this.periodicTransactionId = periodicTransactionId;
			this.state = state;
			this.perPaymentAmount = perPaymentAmount;
		}
		
		public PeriodicPurchaseInfo(long periodicTransactionId, State state, Schedule schedule, long perPaymentAmount,
				string orderId, string customerId, DateTime startDate, DateTime endDate, DateTime nextPaymentDate) {
			this.periodicTransactionId = periodicTransactionId;
			this.state = state;
			this.schedule = schedule;
			this.perPaymentAmount = perPaymentAmount;
			this.orderId = orderId;
			this.customerId = customerId;
			this.startDate = startDate;
			this.endDate = endDate;
			this.nextPaymentDate = nextPaymentDate;
		}
		
		public long getPeriodicTransactionId() {
			return this.periodicTransactionId;
		}
		
		public State getState() {
			return this.state;
		}
		public Schedule getSchedule(){
			return this.schedule;
		}
		
		public long getPerPaymentAmount(){
			return this.perPaymentAmount;
		}
		
		public string getOrderId(){
			return this.orderId;
		}
		
		public string getCustomerId(){
			return this.customerId;
		}
		
		public DateTime getStartDate(){
			return this.startDate;
		}
		
		public DateTime getEndDate(){
			return this.endDate;
		}
		
		public DateTime getNextPaymentDate(){
			return this.nextPaymentDate;
		}
		
		public long getLastPaymentId(){
			return this.lastPaymentId;
		}
		
		//used to display all Dates in a consistent format (yyyy-MM-dd)
		private string formatDate(DateTime unformattedDate){
			string formatted;
			try{
				formatted = unformattedDate.ToString(HttpsCreditCardService.DATE_FORMAT);
				return formatted;
			}catch (Exception e){
				throw e;
			}
		}

		public override String toString() {
			StringBuilder str = new StringBuilder();
			str.Append("[");
			str.Append("periodicTransactionId = ").Append(this.periodicTransactionId).Append(", ");
			str.Append("state = ").Append(this.state.ToString()).Append(", ");
			str.Append("perPaymentAmount = ").Append(this.perPaymentAmount).Append(", ");
			str.Append("orderId = ").Append(this.orderId).Append(", ");
			str.Append("customerId = ").Append(this.customerId).Append(", ");
			str.Append("startDate = ").Append(formatDate(this.startDate)).Append(", ");
			str.Append("endDate = ").Append(formatDate(this.endDate)).Append(", ");
			str.Append("nextPaymentDate = ").Append(formatDate(this.nextPaymentDate)).Append(", ");
			str.Append("lastPaymentId = ").Append(this.lastPaymentId);
			str.Append("]");
			return str.ToString();
		}
	}//end class
}//end namespace
