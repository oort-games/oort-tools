using System.IO;
using UnityEngine;

namespace OortTools
{
    public static class PathUtility
    {
        /// <summary>
        /// 지정된 경로에 중복되지 않는 파일명을 생성하고, 필요 시 디렉토리를 생성합니다.
        /// </summary>
        /// <param name="folderPath">저장할 폴더 경로</param>
        /// <param name="baseName">기본 파일 이름</param>
        /// <param name="extension">확장자 (예: ".jpg" 또는 "jpg")</param>
        /// <returns>중복이 제거된 최종 파일명 (파일명만 리턴)</returns>
        public static string GetUniqueFileName(string folderPath, string baseName, string extension)
        {
            // 1. 폴더가 없으면 생성
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 2. 확장자 포맷 정리 (점'.'이 없으면 추가)
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            // 3. 파일명에서 금지된 문자 제거
            string safeBaseName = baseName;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                safeBaseName = safeBaseName.Replace(c, '_');
            }

            // 4. 중복 확인 및 번호 매기기
            string fileName = $"{safeBaseName}{extension}";
            string fullPath = Path.Combine(folderPath, fileName);
            int counter = 1;

            // 파일이 이미 존재한다면 이름_1, 이름_2 순으로 검색
            while (File.Exists(fullPath))
            {
                fileName = $"{safeBaseName}_{counter}{extension}";
                fullPath = Path.Combine(folderPath, fileName);
                counter++;
            }

            return fileName;
        }
    }
}



