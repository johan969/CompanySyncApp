using System.Text.Json.Serialization;

namespace Arb_Int.Domain.Worker
{
    [JsonConverter(
        typeof(JsonStringEnumConverter<ScheduledTaskType>))]
    /// <summary>
    /// Type of action to perform when the Scheduled task is triggered
    /// </summary>
    public enum ScheduledTaskType : int
    {
        /// <summary>
        /// Invalid task type
        /// </summary>
        None,
        /// <summary>
        /// Send regular email message
        /// </summary>
        EmailMessage,
        /// <summary>
        /// Batch of invitations (multiple emails)
        /// </summary>
        AssessmentInvitationBatch,
        /// <summary>
        /// Split up email invitation from an AssessmentInvitationBatch-task
        /// </summary>
        AssessmentInvitationEmail,
        /// <summary>
        /// Batch of reminders (can be multiple emails depending on how many respondent's remaining unanswered)
        /// </summary>
        AssessmentReminderBatch,
        /// <summary>
        /// Send assessment email reminders
        /// </summary>
        AssessmentReminderEmail,
        /// <summary>
        /// Close an assessment at the deadline
        /// </summary>
        AssessmentDeadline,
        /// <summary>
        /// Renew a license
        /// </summary>
        LicenseRenewal,
        /// <summary>
        /// Cancel a license
        /// </summary>
        LicenseCancel,
        /// <summary>
        /// Update currency rates
        /// </summary>
        CurrencyExchangeRate,
        /// <summary>
        /// ...
        /// </summary>
        ReservedForLaterUse,
        /// <summary>
        /// Generate & send the personal information stored about a user/respondent
        /// </summary>
        EmailPersonalInformationStored,
        /// <summary>
        /// Create invoices
        /// </summary>
        CreateInvoices,
        /// <summary>
        /// Create debit basis
        /// </summary>
        CreateDebitBasis,
        /// <summary>
        /// Cleanup expired/revoked refresh tokens
        /// </summary>
        CleanupExpiredRefreshTokens
    }

    [JsonConverter(
    typeof(JsonStringEnumConverter<ScheduleTriggerType>))]
    /// <summary>
    /// How and when to trigger the Scheduled task
    /// </summary>
    public enum ScheduleTriggerType : int
    {
        /// <summary>
        /// No trigger type
        /// </summary>
        None = 0,
        /// <summary>
        /// Triggers once, as soon as possible
        /// </summary>
        Once = 1,
        /// <summary>
        /// Triggers every minute
        /// </summary>
        EveryMinute = 2,
        /// <summary>
        /// Triggers every 5 minutes
        /// </summary>
        EveryFiveMinutes = 3,
        /// <summary>
        /// Triggers every 10 minutes
        /// </summary>
        EveryTenMinutes = 4,
        /// <summary>
        /// Triggers every 15 minutes
        /// </summary>
        EveryFifteenMinutes = 5,
        /// <summary>
        /// Triggers every 20 minutes
        /// </summary>
        EveryTwentyMinutes = 6,
        /// <summary>
        /// Triggers every 30 minutes
        /// </summary>
        EveryHalfHour = 7,
        /// <summary>
        /// Triggers every 45 minutes
        /// </summary>
        EvertFourtyFiveMinutes = 8,
        /// <summary>
        /// Triggers every hour
        /// </summary>
        EveryHour = 9,
        /// <summary>
        /// Triggers every two hours
        /// </summary>
        EveryTwoHours = 10,
        /// <summary>
        /// Triggers every three hours
        /// </summary>
        EveryThreeHours = 11,
        /// <summary>
        /// Triggers every six hours
        /// </summary>
        EverySixHours = 12,
        /// <summary>
        /// Triggers every twelve hours
        /// </summary>
        EveryTwelveHours = 13,
        /// <summary>
        /// Triggers once per day
        /// </summary>
        Daily = 14,
        /// <summary>
        /// Triggers every monday
        /// </summary>
        EveryMonday = 15,
        /// <summary>
        /// Triggers every tuesday
        /// </summary>
        EveryTuesday = 16,
        /// <summary>
        /// Triggers every wednesday
        /// </summary>
        EveryWednesday = 17,
        /// <summary>
        /// Triggers every thirsday
        /// </summary>
        EveryThirsday = 18,
        /// <summary>
        /// Triggers every friday
        /// </summary>
        EveryFriday = 19,
        /// <summary>
        /// Triggers every saturday
        /// </summary>
        EverySaturday = 20,
        /// <summary>
        /// Triggers every sunday
        /// </summary>
        EverySunday = 21,
        /// <summary>
        /// Triggers every weekday (monday, tuesday, wednesday, thirsday, friday)
        /// </summary>
        EveryWeekday = 22,
        /// <summary>
        /// Triggers once a week
        /// </summary>
        Weekly = 23,
        /// <summary>
        /// Triggers once a month
        /// </summary>
        Monthly = 24,
        /// <summary>
        /// Triggers once every quarter
        /// </summary>
        Quarterly = 25,
        /// <summary>
        /// Triggers once every six months
        /// </summary>
        HalfAYear = 26,
        /// <summary>
        /// Triggers once a year
        /// </summary>
        Yearly = 27,
        /// <summary>
        /// 1st day of the month
        /// </summary>
        DayTheMonth1st = 28,
        /// <summary>
        /// 2nd day of the month
        /// </summary>
        DayfTheMonth2nd = 29,
        /// <summary>
        /// 3rd day of the month
        /// </summary>
        DayfTheMonth3rd = 30,
        /// <summary>
        /// 4th day of the month
        /// </summary>
        DayfTheMonth4th = 31,
        /// <summary>
        /// 5th day of the month
        /// </summary>
        DayfTheMonth5th = 32,
        /// <summary>
        /// 6th day of the month
        /// </summary>
        DayfTheMonth6th = 33,
        /// <summary>
        /// 7th day of the month
        /// </summary>
        DayfTheMonth7th = 34,
        /// <summary>
        /// 8th day of the month
        /// </summary>
        DayfTheMonth8th = 35,
        /// <summary>
        /// 9th day of the month
        /// </summary>
        DayfTheMonth9th = 36,
        /// <summary>
        /// 10th day of the month
        /// </summary>
        DayfTheMonth10th = 37,
        /// <summary>
        /// 11th day of the month
        /// </summary>
        DayfTheMonth11th = 38,
        /// <summary>
        /// 12th day of the month
        /// </summary>
        DayfTheMonth12th = 39,
        /// <summary>
        /// 13th day of the month
        /// </summary>
        DayfTheMonth13th = 40,
        /// <summary>
        /// 14th day of the month
        /// </summary>
        DayfTheMonth14th = 41,
        /// <summary>
        /// 15th day of the month
        /// </summary>
        DayfTheMonth15th = 42,
        /// <summary>
        /// 16th day of the month
        /// </summary>
        DayfTheMonth16th = 43,
        /// <summary>
        /// 17th day of the month
        /// </summary>
        DayfTheMonth17th = 44,
        /// <summary>
        /// 18th day of the month
        /// </summary>
        DayfTheMonth18th = 45,
        /// <summary>
        /// 19th day of the month
        /// </summary>
        DayfTheMonth19th = 46,
        /// <summary>
        /// 20th day of the month
        /// </summary>
        DayfTheMonth20th = 47,
        /// <summary>
        /// 21th day of the month
        /// </summary>
        DayfTheMonth21th = 48,
        /// <summary>
        /// 22th day of the month
        /// </summary>
        DayfTheMonth22th = 49,
        /// <summary>
        /// 23th day of the month
        /// </summary>
        DayfTheMonth23th = 50,
        /// <summary>
        /// 24th day of the month
        /// </summary>
        DayfTheMonth24th = 51,
        /// <summary>
        /// 25th day of the month
        /// </summary>
        DayfTheMonth25th = 52,
        /// <summary>
        /// 26th day of the month
        /// </summary>
        DayfTheMonth26th = 53,
        /// <summary>
        /// 27th day of the month
        /// </summary>
        DayfTheMonth27th = 54,
        /// <summary>
        /// 28th day of the month
        /// </summary>
        DayfTheMonth28th = 55,
        /// <summary>
        /// 29th day of the month (it it exists, otherwise the last day of the month will be used)
        /// </summary>
        DayfTheMonth29th = 56,
        /// <summary>
        /// 30th day of the month (it it exists, otherwise the last day of the month will be used)
        /// </summary>
        DayfTheMonth30th = 57,
        /// <summary>
        /// 31th day of the month (it it exists, otherwise the last day of the month will be used)
        /// </summary>
        DayfTheMonth31th = 58,
    }

    [JsonConverter(
    typeof(JsonStringEnumConverter<ScheduledTaskStatus>))]
    /// <summary>
    /// The status of the Scheduled task
    /// </summary>
    public enum ScheduledTaskStatus
    {
        /// <summary>
        /// Pending status
        /// </summary>
        Pending,
        /// <summary>
        /// Is being dispatched (Email)
        /// </summary>
        Dispatching,
        /// <summary>
        /// Is being processed
        /// </summary>
        Processing,
        /// <summary>
        /// Has been enqued to Hangfire
        /// </summary>
        Enqueued,
        /// <summary>
        /// Completed successfully
        /// </summary>
        Succeeded,
        /// <summary>
        /// Failed status
        /// </summary>
        Failed,
        /// <summary>
        /// Dead status
        /// </summary>
        Dead
    }

    [JsonConverter(
    typeof(JsonStringEnumConverter<ScheduledTaskPriority>))]
    /// <summary>
    /// The priority of the Scheduled task
    /// </summary>
    public enum ScheduledTaskPriority
    {
        None = 0,
        Low = 1,
        Normal = 2,
        High = 3,
    }

    [JsonConverter(
    typeof(JsonStringEnumConverter<ScheduleTimeSemantics>))]
    public enum ScheduleTimeSemantics
    {
        /// <summary>
        /// Triggers are fixed in UTC, next = +interval in UTC.
        /// </summary>
        UtcFixed = 0,

        /// <summary>
        /// Triggers are interpreted in the user's time zone (wall clock), next computed in local time then converted to UTC.
        /// DST-safe.
        /// </summary>
        UserLocalWallClock = 1
    }

    [JsonConverter(
    typeof(JsonStringEnumConverter<ScheduledTaskDefinitionStatus>))]
    public enum ScheduledTaskDefinitionStatus
    {
        Enabled = 0,
        Disabled = 1,
        Completed = 2
    }
}