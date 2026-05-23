import json
import os
import re

def grade_run(run_dir, metadata):
    results = []
    for assertion in metadata['assertions']:
        name = assertion['name']
        passed = False
        evidence = ""
        
        if assertion['type'] == 'file_exists':
            path = os.path.join(run_dir, 'outputs', assertion['path'])
            passed = os.path.exists(path)
            evidence = f"File {assertion['path']} exists" if passed else f"File {assertion['path']} does not exist"
        
        elif assertion['type'] == 'contains':
            path = os.path.join(run_dir, 'outputs', assertion['path'])
            if os.path.exists(path):
                with open(path, 'r') as f:
                    content = f.read()
                    if re.search(assertion['pattern'], content):
                        passed = True
                        evidence = f"Found pattern '{assertion['pattern']}' in {assertion['path']}"
                    else:
                        evidence = f"Did not find pattern '{assertion['pattern']}' in {assertion['path']}"
            else:
                evidence = f"File {assertion['path']} does not exist"
        
        results.append({
            "text": name,
            "passed": passed,
            "evidence": evidence
        })
    
    with open(os.path.join(run_dir, 'grading.json'), 'w') as f:
        json.dump({"expectations": results}, f, indent=2)

def main():
    iteration_dir = "/home/dinh/Rate/dotnet-clean-architecture-jasontaylor-workspace/iteration-2"
    for eval_dir in os.listdir(iteration_dir):
        eval_path = os.path.join(iteration_dir, eval_dir)
        if not os.path.isdir(eval_path): continue
        
        metadata_path = os.path.join(eval_path, 'eval_metadata.json')
        if not os.path.exists(metadata_path): continue
        
        with open(metadata_path, 'r') as f:
            metadata = json.load(f)
        
        for run_type in ['with_skill', 'without_skill']:
            run_dir = os.path.join(eval_path, run_type)
            if os.path.exists(run_dir):
                grade_run(run_dir, metadata)

if __name__ == "__main__":
    main()
