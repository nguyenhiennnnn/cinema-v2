import os
import re
import sys

# Ensure UTF-8 output
sys.stdout.reconfigure(encoding='utf-8', errors='backslashreplace')

IGNORE_DIRS = {
    'node_modules', '.git', '.system_generated', 'bin', 'obj', 'dist', 
    'build', '.vscode', '.agents', 'test-results', 'coverage', 'results'
}

EXTENSIONS = {
    '.cs', '.ts', '.tsx', '.js', '.jsx', '.json', '.html', '.md', '.ps1', '.sh', '.resx'
}

# Misdecoded UTF-8 patterns
MOJIBAKE_REGEX = re.compile(r'á[º»][\x80-\xff]|Ä[‘“]|Ã[¡àã²óùúýâêô]|Æ[°¡]|Káº|quáº|kiá»|thá»|Ä‘Ã')
REPLACEMENT_CHAR = '\ufffd'
QUESTION_MARK_WORD = re.compile(r'\b[a-zA-Z\u00C0-\u1EF9]*\?[a-zA-Z\u00C0-\u1EF9]+\b|\b[a-zA-Z\u00C0-\u1EF9]+\?[a-zA-Z\u00C0-\u1EF9]*\b')

findings = []

for root, dirs, files in os.walk('.'):
    dirs[:] = [d for d in dirs if d not in IGNORE_DIRS]
    for file in files:
        ext = os.path.splitext(file)[1].lower()
        if ext not in EXTENSIONS:
            continue
        filepath = os.path.join(root, file)
        try:
            with open(filepath, 'r', encoding='utf-8', errors='replace') as f:
                for line_no, line in enumerate(f, 1):
                    # Check for replacement char
                    if REPLACEMENT_CHAR in line:
                        findings.append((filepath, line_no, "REPLACEMENT_CHAR", line.strip()))
                    # Check for mojibake
                    elif MOJIBAKE_REGEX.search(line):
                        findings.append((filepath, line_no, "MOJIBAKE", line.strip()))
        except Exception as e:
            pass

print(f"Total findings: {len(findings)}")
for path, lno, kind, text in findings:
    print(f"{path}:{lno} [{kind}] {text[:120]}")
