import os, re, json, yaml, urllib.request
from datetime import datetime, timezone

PR_BODY   = os.environ.get('PR_BODY', '') or ''
PR_NUMBER = os.environ.get('PR_NUMBER', '')
PR_URL    = os.environ.get('PR_URL', '')
PR_AUTHOR = os.environ.get('PR_AUTHOR', '')
WEBHOOK   = os.environ['DISCORD_WEBHOOK']
PATH      = 'Resources/Changelog/Changelog.yml'

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
req = urllib.request.Request(
    WEBHOOK,
    data=json.dumps({'embeds': [embed]}).encode('utf-8'),
    headers={'Content-Type': 'application/json'},
)
urllib.request.urlopen(req)
print('Posted to Discord')
