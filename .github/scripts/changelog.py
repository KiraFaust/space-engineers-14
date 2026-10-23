import os, re, json, yaml, urllib.request
from datetime import datetime, timezone

PR_BODY = os.environ.get('PR_BODY', '') or ''
PR_NUMBER = '1'
PR_URL = 'http://example.com/pr/1'
PR_AUTHOR = "KiraFaust"
WEBHOOK = os.environ['DISCORD_WEBHOOK']
from pathlib import Path
REPO_ROOT = Path(__file__).resolve().parents[2]
PATH = REPO_ROOT / 'Resources/Changelog/Changelog.yml'

m = re.search(r':cl:\s*(.*?)(?:\n\s*\n|\Z)', PR_BODY, re.DOTALL | re.IGNORECASE)
if not m:
    print('No :cl: in PR body — skip'); raise SystemExit(0)

body = m.group(1).strip()
if re.fullmatch(r'no\b.*', body, re.IGNORECASE):
    print(':cl: no — skip'); raise SystemExit(0)

type_map = {'add': 'Add', 'del': 'Remove', 'remove': 'Remove',
            'tweak': 'Tweak', 'fix': 'Fix'}
changes = []
for line in body.splitlines():
    mm = re.match(r'-\s*(\w+):\s*(.+)', line.strip())
    if mm:
        t = type_map.get(mm.group(1).lower(), mm.group(1).capitalize())
        changes.append({'type': t, 'message': mm.group(2).strip()})

if not changes:
    print('No valid entries — skip'); raise SystemExit(0)

data = {}
if os.path.exists(PATH):
    with open(PATH, encoding='utf-8') as f:
        data = yaml.safe_load(f) or {}
data.setdefault('Entries', [])

data['Entries'].insert(0, {
    'author': PR_AUTHOR,
    'changes': changes,
    'id': int(PR_NUMBER),
    'time': datetime.now(timezone.utc).isoformat(),
    'url': PR_URL,
})


with open(PATH, 'w', encoding='utf-8') as f:
    yaml.safe_dump(data, f, allow_unicode=True,
                   sort_keys=False, default_flow_style=False)

embed = {
    'title': f'PR #{PR_NUMBER}',
    'url': PR_URL,
    'description': '\n'.join(f"**{c['type']}**: {c['message']}" for c in changes),
    'footer': {'text': f'Author: {PR_AUTHOR}'},
}
headers={
    'Content-Type': 'application/json',
    'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)',
    'Accept': 'application/json',
}
req = urllib.request.Request(
    WEBHOOK,
    data=json.dumps({'embeds': [embed]}).encode('utf-8'),
    headers={'Content-Type': 'application/json'},
)

req = urllib.request.Request(
    WEBHOOK,
    data=json.dumps({'embeds': [embed]}).encode('utf-8'),
    headers={
        'Content-Type': 'application/json',
        'User-Agent': 'SS14-Changelog/1.0 (https://github.com/YourOrg/YourRepo, 1.0)',
    },
)

urllib.request.urlopen(req)
print('Posted to Discord')
