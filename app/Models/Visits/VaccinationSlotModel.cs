﻿namespace backend.Models.Visits
{
    public class VaccinationSlotModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool Reserved { get; set; }
    }
}