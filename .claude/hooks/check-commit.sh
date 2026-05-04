#!/bin/bash
if ! git diff-index --quiet HEAD --; then
  echo "有未提交的更改，请检查并拆分提交。"
  exit 1
fi
