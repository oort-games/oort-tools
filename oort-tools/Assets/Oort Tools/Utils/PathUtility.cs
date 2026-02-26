using System.Collections.Generic;
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
        /// <param name="reservedPaths">이미 예약된 전체 경로 목록 (Hashset 등)</param>
        /// <returns>중복이 제거된 최종 파일명 (파일명만 리턴)</returns>
        public static string GetUniqueFileName(string folderPath, string baseName, string extension, IEnumerable<string> reservedPaths = null)
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
            int counter = 0;
            while (true)
            {
                string suffix = (counter == 0) ? "" : $"_{counter}";
                string fileName = $"{safeBaseName}{suffix}{extension}";
                string fullPath = Path.Combine(folderPath, fileName);

                bool existsOnDisk = File.Exists(fullPath);

                bool isReserved = false;
                if (reservedPaths != null)
                {
                    foreach (var path in reservedPaths)
                    {
                        if (string.Equals(path, fullPath, System.StringComparison.OrdinalIgnoreCase))
                        {
                            isReserved = true;
                            break;
                        }
                    }
                }

                // 둘 다 해당사항이 없으면 이 이름으로 결정
                if (!existsOnDisk && !isReserved)
                {
                    return fileName;
                }

                counter++;
            }
        }
    }
}



