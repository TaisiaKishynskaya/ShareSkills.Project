﻿namespace Libraries.Entities.Concrete;

public class TimeOfDayEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }

    //1-*
    public List<TeacherEntity> Teachers { get; set; }
}
