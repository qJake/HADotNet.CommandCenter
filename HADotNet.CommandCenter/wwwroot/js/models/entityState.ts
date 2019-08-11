type AttributeDictionary = { [key: string]: any; };

interface IEntityState
{
    /** Gets or sets the Entity ID that this state represents. */
    entityId: string

    /** Gets or sets the string representation of the state that this entity is currently in. */
    state: string

    /** Gets or sets the entity's current attributes and values. */
    attributes: AttributeDictionary

    /** Gets or sets the UTC date and time that this state was last changed. */
    lastChanged: Date

    /** Gets or sets the UTC date and time that this state was last updated. */
    lastUpdated: Date
}