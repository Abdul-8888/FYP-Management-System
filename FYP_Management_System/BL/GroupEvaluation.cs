using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FYP_Management_System.BL
{
    public class GroupEvaluation
    {
        private int groupId;
        private int evaluationId;
        private int obtainedMarks;
        private string date;

        public int GroupId { get => groupId; set => groupId = value; }
        public int EvaluationId { get => evaluationId; set => evaluationId = value; }
        public int ObtainedMarks { get => obtainedMarks; set => obtainedMarks = value; }
        public string Date { get => date; set => date = value; }

        public GroupEvaluation(int groupId, int evaluationId, int obtainedMarks, string date)
        {
            GroupId = groupId;
            EvaluationId = evaluationId;
            ObtainedMarks = obtainedMarks;
            Date = date;
        }
    }
}
