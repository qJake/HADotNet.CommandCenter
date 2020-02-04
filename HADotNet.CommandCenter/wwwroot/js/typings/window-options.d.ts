/// <reference path="../models/models.ts" />

declare interface ICcOptions
{
    baseUrl: string;
    overrideAssetUrl: string;
    token: string;
    tilePadding: number;
    tilePreviewPadding: number;
    tileSize: number;
    tilePreviewSize: number;
    mode: PageMode;
    pageId: string;
    autoReturn: number;
}

declare interface Window
{
    ccOptions: ICcOptions;
}
