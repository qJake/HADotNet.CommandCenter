/// <reference path="entityState.ts" />

enum PageMode
{
    User = 0,
    Admin = 1
}

type StateDictionary = { [name: string]: IEntityState };
type StringDictionary = { [name: string]: string };