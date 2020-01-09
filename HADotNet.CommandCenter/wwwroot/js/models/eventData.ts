interface ICalendarEvent
{
    attendees?: null;
    created: string;
    creator: ICalendarPerson;
    end: IDateTime;
    etag: string;
    htmlLink: string;
    iCalUID: string;
    id: string;
    kind: string;
    location: string;
    organizer: ICalendarPerson;
    originalStartTime?: null;
    recurringEventId?: null;
    reminders: IReminders;
    sequence: number;
    start: IDateTime;
    status: string;
    summary: string;
    transparency?: null;
    updated: string;
}
interface ICalendarPerson
{
    displayName?: null;
    email: string;
    responseStatus?: null;
    self: boolean;
    organizer?: null;
}
interface IDateTime
{
    dateTime: string;
    date: string;
    timeZone?: null;
}
interface IReminders
{
    useDefault: boolean;
    overrides?: (IOverridesEntity)[] | null;
}
interface IOverridesEntity
{
    method: string;
    minutes: number;
}
