﻿using System;
using System.Collections.Generic;

namespace HRMS.Data.General
{
    public partial class Evaluation
    {
        public Evaluation()
        {
            EvaluationDocument = new HashSet<EvaluationDocument>();
            EvaluationManager = new HashSet<EvaluationManager>();
            EvaluationQuestionnaireNumerical = new HashSet<EvaluationQuestionnaireNumerical>();
            EvaluationQuestionnaireOptional = new HashSet<EvaluationQuestionnaireOptional>();
            EvaluationQuestionnaireTopic = new HashSet<EvaluationQuestionnaireTopic>();
            EvaluationSelf = new HashSet<EvaluationSelf>();
            EvaluationStatus = new HashSet<EvaluationStatus>();
            EvaluationStudentsCollege = new HashSet<EvaluationStudentsCollege>();
            EvaluationStudentsStaff = new HashSet<EvaluationStudentsStaff>();
        }

        public int EvaluationId { get; set; }
        public int EvaluationTypeId { get; set; }
        public string InsertedFrom { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedFrom { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedNo { get; set; }

        public virtual EvaluationType EvaluationType { get; set; }
        public virtual AspNetUsers InsertedFromNavigation { get; set; }
        public virtual AspNetUsers UpdatedFromNavigation { get; set; }
        public virtual ICollection<EvaluationDocument> EvaluationDocument { get; set; }
        public virtual ICollection<EvaluationManager> EvaluationManager { get; set; }
        public virtual ICollection<EvaluationQuestionnaireNumerical> EvaluationQuestionnaireNumerical { get; set; }
        public virtual ICollection<EvaluationQuestionnaireOptional> EvaluationQuestionnaireOptional { get; set; }
        public virtual ICollection<EvaluationQuestionnaireTopic> EvaluationQuestionnaireTopic { get; set; }
        public virtual ICollection<EvaluationSelf> EvaluationSelf { get; set; }
        public virtual ICollection<EvaluationStatus> EvaluationStatus { get; set; }
        public virtual ICollection<EvaluationStudentsCollege> EvaluationStudentsCollege { get; set; }
        public virtual ICollection<EvaluationStudentsStaff> EvaluationStudentsStaff { get; set; }
    }
}
