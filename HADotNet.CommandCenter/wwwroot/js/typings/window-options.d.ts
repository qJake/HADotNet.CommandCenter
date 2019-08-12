/// <reference path="../models/models.ts" />

declare interface ICcOptions
{
    tilePadding: number;
    tilePreviewPadding: number;
    tileSize: number;
    tilePreviewSize: number;
    mode: PageMode;
}

declare interface Window
{
    ccOptions: ICcOptions;
}
