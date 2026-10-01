// Backend only has UploadImageResponse (used for both image and video upload)
export interface UploadImageResponse {
  url: string;
  publicId: string;
}
