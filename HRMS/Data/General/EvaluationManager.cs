﻿using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class EvaluationManager
    {
        public EvaluationManager()
        {
            EvaluationManagerDocument = new HashSet<EvaluationManagerDocument>();
            EvaluationManagerQuestionnaire = new HashSet<EvaluationManagerQuestionnaire>();
        }

        public int EvaluationManagerId { get; set; }
        public int EvalutaionId { get; set; }
        public int ManagerId { get; set; }
        public int StaffId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual Evaluation Evalutaion { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual StaffDepartment Manager { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationManagerDocument> EvaluationManagerDocument { get; set; }
        public virtual ICollection<EvaluationManagerQuestionnaire> EvaluationManagerQuestionnaire { get; set; }
    }
}
